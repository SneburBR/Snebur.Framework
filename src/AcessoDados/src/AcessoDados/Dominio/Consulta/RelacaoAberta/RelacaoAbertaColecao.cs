namespace Snebur.AcessoDados;

public class RelacaoAbertaColecao : BaseRelacaoAberta
{
    #region Campos Privados

    #endregion

    public EstruturaConsulta? EstruturaConsulta { get; set; }

    public RelacaoAbertaColecao() : base()
    {
    }

    [IgnorarConstrutorTS]
    public RelacaoAbertaColecao(PropertyInfo propriedade) : base(propriedade)
    {
    }
}