namespace Snebur.AcessoDados.Estrutura;

internal class EstruturaRelacaoNn : EstruturaRelacao
{

    internal EstruturaEntidade EstruturaEntidadeRelacaoNn { get; }

    internal EstruturaCampo EstruturaCampoChaveEstrangeiraPai { get; }

    internal EstruturaCampo EstruturaCampoChaveEstrangeiraFilho { get; }

    internal EstruturaEntidade EstruturaEntidadePai { get; }

    internal EstruturaEntidade EstruturaEntidadeFilho { get; }

    internal EstruturaRelacaoPai? EstruturaRelacaoPaiEntidadePai { get; set; }

    internal EstruturaRelacaoPai? EstruturaRelacaoPaiEntidadeFilho { get; set; }

    internal EstruturaRelacaoNn(
        PropertyInfo propriedade,
        EstruturaEntidade estruturaEntidadeRelacaoNn,
        EstruturaEntidade estruturaEntidadePai,
        EstruturaEntidade estruturaEntidadeFilho,
        EstruturaCampo estruturaCampoChaveEstrangeiraPai,
        EstruturaCampo estruturaCampoChaveEstrangeiraFilho)
        : base(propriedade, estruturaEntidadePai)
    {
        this.EstruturaEntidadeRelacaoNn = estruturaEntidadeRelacaoNn;
        this.EstruturaEntidadePai = estruturaEntidadePai;
        this.EstruturaEntidadeFilho = estruturaEntidadeFilho;

        this.EstruturaCampoChaveEstrangeiraPai = estruturaCampoChaveEstrangeiraPai;
        this.EstruturaCampoChaveEstrangeiraFilho = estruturaCampoChaveEstrangeiraFilho;

        if (this.EstruturaCampoChaveEstrangeiraPai == this.EstruturaCampoChaveEstrangeiraFilho)
        {
            throw new InvalidOperationException();
        }
    }
}