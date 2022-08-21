using System.Collections.Generic;
using Snebur.AcessoDados.Estrutura;
using Snebur.Dominio.Atributos;

namespace Snebur.AcessoDados
{
    internal class SqlValidacaoUnico : BaseSqlIndice
    {
        internal SqlValidacaoUnico(EstruturaEntidade estruturaEntidade, PropriedadeIndexar propriedade) : 
            this(estruturaEntidade,
                new List<PropriedadeIndexar>() { propriedade },
                new List<FiltroPropriedadeIndexar>())
        {
        }

        internal SqlValidacaoUnico(EstruturaEntidade estruturaEntidade, 
                                   List<PropriedadeIndexar> propriedades,
                                   List<FiltroPropriedadeIndexar> filtros) :
            base(estruturaEntidade, propriedades, filtros,true)
        {
        }
    }
}