using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using Snebur.AcessoDados.Dominio;

namespace Snebur.AcessoDados
{
    public static class ConsultaEntidadeExtensao
    {
        public static string RetornarSql(this BaseConsultaEntidade consultaEntidade)
        {
            return ((BaseContextoDados)consultaEntidade.ContextoDados).RetornarSql(consultaEntidade.EstruturaConsulta);
        }
    }
}
