using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using System.Linq.Expressions;

namespace Snebur.AcessoDados.Ajudantes
{
    internal partial class AjudanteFiltroPropriedade
    {

        public static FiltroPropriedade RetornarFiltroPropriedadeVardadeiro(EstruturaConsulta estruturaConsulta, MemberExpression expressao)
        {
            var valorPropriedade = true;
            var operadorFiltro = EnumOperadorFiltro.Igual;
            return AjudanteFiltroPropriedade.RetornarFiltroPropriedade(estruturaConsulta, expressao, valorPropriedade, operadorFiltro);
        }
    }
}