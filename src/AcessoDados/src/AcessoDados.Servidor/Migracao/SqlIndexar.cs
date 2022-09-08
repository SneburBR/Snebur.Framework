using Snebur.AcessoDados.Estrutura;
using Snebur.Dominio.Atributos;
using System.Collections.Generic;

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