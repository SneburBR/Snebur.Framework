using System;
using System.Reflection;

namespace Snebur.AcessoDados.Estrutura
{
    internal class EstruturaRelacaoNn : EstruturaRelacao
    {

        internal EstruturaEntidade EstruturaEntidadeRelacaoNn { get; set; }

        internal EstruturaCampo EstruturaCampoChaveEstrangeiraPai { get; set; }

        internal EstruturaCampo EstruturaCampoChaveEstrangeiraFilho { get; set; }

        internal EstruturaEntidade EstruturaEntidadePai { get; set; }

        internal EstruturaEntidade EstruturaEntidadeFilho { get; set; }

        internal EstruturaRelacaoPai EstruturaRelacaoPaiEntidadePai { get; set; }

        internal EstruturaRelacaoPai EstruturaRelacaoPaiEntidadeFilho { get; set; }

        internal EstruturaRelacaoNn(PropertyInfo propriedade,
                                    EstruturaEntidade estruturaEntidadeRelacaoNn,
                                    EstruturaEntidade estruturaEntidadePai,
                                    EstruturaEntidade estruturaEntidadeFilho,
                                    EstruturaCampo estruturaCampoChaveEstrangeiraPai,
                                    EstruturaCampo estruturaCampoChaveEstrangeiraFilho) : base(propriedade, estruturaEntidadePai)
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
}