namespace Snebur.AcessoDados.Seguranca;

internal class EstruturaRestricaoFiltro
{
    internal IRestricaoFiltroPropriedade RestricaoFiltro { get; set; }

    internal EstruturaRestricaoFiltro(IRestricaoFiltroPropriedade restricao)
    {
        this.RestricaoFiltro = restricao;
    }
}
