using System.Threading.Tasks;

namespace Snebur.Comunicacao;

public class ChamadaServicoAsync : BaseChamadaServico
{
    private Action<ArgsResultadoChamadaServico>? _callback;
    private object? _userState;

    public ChamadaServicoAsync(string nomeManipulador,
                               ContratoChamada informacaoChamada,
                               string urlServico,
                               Type tipoRetorno,
                               Dictionary<string, string>? parametrosCabeacalhoAdicionais) :
                               base(nomeManipulador, informacaoChamada, urlServico, tipoRetorno, parametrosCabeacalhoAdicionais)
    {
    }

    public void ExecutarChamaraAsync(
        Action<ArgsResultadoChamadaServico>? callback,
        object? userState = null)
    {
        this._callback = callback;
        this._userState = userState;
        Task.Factory.StartNew(this.ExecutarChamada);
    }

    private void ExecutarChamada()
    {
        object? resultado = null;
        Exception? erro = null;
        try
        {
            resultado = this.RetornarValorChamada();
        }
        catch (Exception ex)
        {
            erro = ex;
        }
        finally
        {
            if (this._callback != null)
            {
                var args = new ArgsResultadoChamadaServico(erro, resultado, this._userState);
                this._callback.Invoke(args);
                this._callback = null;
            }
        }
    }
}
