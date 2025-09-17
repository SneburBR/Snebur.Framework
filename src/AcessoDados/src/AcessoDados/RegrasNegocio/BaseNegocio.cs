namespace Snebur.RegrasNegocio;

public abstract class BaseNegocio
{

}

public abstract class BaseNegocio<TContextoDados> : BaseNegocio
{
    public TContextoDados ContextoDados { get; }

    public BaseNegocio(TContextoDados contexto)
    {
        Guard.NotNull(contexto);
        this.ContextoDados = contexto;
    }
}
