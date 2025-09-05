namespace Snebur.Comunicacao;

public class ChamadaServico<TResult> : BaseChamadaServico
{

    public ChamadaServico(string nomeManipulador,
                          ContratoChamada contratoChamada,
                          string urlServico,
                          Type tipoRetorno,
                          Dictionary<string, string>? parametrosCabeacalhoAdicionais) :
                          base(nomeManipulador, contratoChamada, urlServico, tipoRetorno, parametrosCabeacalhoAdicionais)
    {
    }

    public TResult ExecutarChamada()
    {
        return (TResult)this.RetornarValorChamada()!;
    }
}
