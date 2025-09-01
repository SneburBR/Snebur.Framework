namespace Snebur.Depuracao;

public class Contrato : BaseDominio
{

    #region Campos Privados

    #endregion

    public required Mensagem Mensagem { get; set; }

    [IgnorarConstrutorTS]
    public Contrato()
    {

    }

    public Contrato(Mensagem mensagem)
    {
        this.Mensagem = mensagem;
    }
}