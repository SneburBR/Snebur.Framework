namespace Snebur.AcessoDados.Estrutura
{
    internal class EstruturaRelacaoFilhos : EstruturaRelacao
    {

        internal EstruturaEntidade EstruturaEntidadeFilho { get; set; }

        internal EstruturaCampo EstruturaCampoChaveEstrangeira { get; set; }

        internal EstruturaRelacaoPai EstruturaRelacaoPai { get; set; }

        internal EstruturaRelacaoFilhos(PropertyInfo propriedade,
                                        EstruturaEntidade estruturaEntidade,
                                        EstruturaEntidade estruturaEntidadeFilho,
                                        EstruturaCampo estruturaCampoChaveEstrangeira) : base(propriedade, estruturaEntidade)
        {
            this.EstruturaEntidadeFilho = estruturaEntidadeFilho;
            this.EstruturaCampoChaveEstrangeira = estruturaCampoChaveEstrangeira;
        }
    }
}
