using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados
{
    internal class SqlIndexar : BaseSqlIndice
    {

        internal SqlIndexar(EstruturaEntidade estruturaEntidade, PropriedadeIndexar propriedade) :
                            this(estruturaEntidade, new List<PropriedadeIndexar> { propriedade })
        {
        }

        internal SqlIndexar(EstruturaEntidade estruturaEntidade,
                            List<PropriedadeIndexar> propriedades) :
                            base(estruturaEntidade, propriedades, null, false)
        {
        }
    }
}