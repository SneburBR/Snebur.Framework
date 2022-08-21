using System.Reflection;

namespace Snebur.AcessoDados.Estrutura
{
    internal class EstruturaRelacaoPai : EstruturaRelacaoChaveEstrangeira
    {
        internal EstruturaRelacaoPai(PropertyInfo propriedade,
                                     EstruturaEntidade estruturaEntidade,
                                     EstruturaEntidade estruturaEntidadePai,
                                     EstruturaCampo estrutaCampoChaveEstrangeira) :
                                     base(propriedade, estruturaEntidade, estruturaEntidadePai, estrutaCampoChaveEstrangeira)
        {
        }
    }
}