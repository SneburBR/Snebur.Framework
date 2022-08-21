using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;

namespace Snebur.AcessoDados.Mapeamento
{
    internal class IdTipoEntidade : INomeTipoEntidade
    {

        public long Id { get; set; }

        public string __NomeTipoEntidade { get; set; }

        public long CampoFiltro { get; set; }

        //string INomeTipoEntidade.__NomeTipoEntidade => throw new NotImplementedException();
    }
}
