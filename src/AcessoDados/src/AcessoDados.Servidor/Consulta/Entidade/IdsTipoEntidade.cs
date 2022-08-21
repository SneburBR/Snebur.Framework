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
    internal class GrupoIdTipoEntidade
    {
        public SortedSet<long> Ids { get; set; }

        public string NomeTipoEntidade { get; set; }

        public GrupoIdTipoEntidade(SortedSet<long> ids, string nomeTipoEntidade)
        {
            this.Ids = ids;
            this.NomeTipoEntidade = nomeTipoEntidade;
        }
    }
}