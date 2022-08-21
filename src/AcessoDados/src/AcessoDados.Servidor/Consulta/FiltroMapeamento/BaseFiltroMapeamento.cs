using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Mapeamento
{
    internal abstract class BaseFiltroMapeamento
    {
        internal bool IsIdTipoEntidade { get; set; } = false;

        public EstruturaCampo EstruturaCampoFiltro { get; set; }

        internal BaseFiltroMapeamento FiltroMapeamentoBase { get; set; }

        internal BaseFiltroMapeamento(BaseFiltroMapeamento filtroMapaementoBase)
        {
            this.FiltroMapeamentoBase = filtroMapaementoBase;
        }
    }
}