namespace Snebur.AcessoDados;

public abstract partial class BaseContextoDados : __BaseContextoDados, IServicoDados, IContextoDadosSemNotificar, IDisposable
{
    private static bool? __isPopularBanco = null;
    private static readonly object __bloqueioPoularBanco = new object();
    private static Dictionary<Type, bool> __dictPopularBancoPendente = new Dictionary<Type, bool>();

    private static bool IsPopularBanco
    {
        get
        {
            if (!__isPopularBanco.HasValue)
            {
                __isPopularBanco = ConverterUtil.ParaBoolean(AplicacaoSnebur.AtualRequired.AppSettings["IsPopularBanco"]);
            }
            return __isPopularBanco.Value;
        }
    }

    public bool __isPopularBancoPendente
    {
        get
        {
            if (!IsPopularBanco)
            {
                return false;
            }

            if (!__dictPopularBancoPendente.ContainsKey(this.GetType()))
            {
                __dictPopularBancoPendente.Add(this.GetType(), true);
            }
            return __dictPopularBancoPendente[this.GetType()];
        }
        set
        {
            __dictPopularBancoPendente.AddOrUpdate(this.GetType(), value);
        }
    }

    private void PopularBancoInterno()
    {
        lock (__bloqueioPoularBanco)
        {
            if (this.__isPopularBancoPendente)
            {
                this.OnPopularBanco();
                this.__isPopularBancoPendente = false;
            }
        }
    }

    protected virtual void OnPopularBanco()
    {

    }
}
