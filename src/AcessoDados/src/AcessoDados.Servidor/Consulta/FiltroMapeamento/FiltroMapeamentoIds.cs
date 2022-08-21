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
    internal class FiltroMapeamentoIds : BaseFiltroMapeamento
    {

        internal SortedSet<long> Ids { get; }
        
        internal FiltroMapeamentoIds(BaseFiltroMapeamento filtroMapeamentoBase,
                                   SortedSet<long> ids) : base(filtroMapeamentoBase)
        {
            this.Ids = ids;
        }

        internal FiltroMapeamentoIds(SortedSet<long> ids) : base(new FiltroMapeamentoVazio())
        {
            this.Ids = ids;
        }

        internal FiltroMapeamentoIds(EstruturaCampo estruturaCampoFiltro, SortedSet<long> ids) : base(new FiltroMapeamentoVazio())
        {
            this.EstruturaCampoFiltro = estruturaCampoFiltro;
            this.Ids = ids;
        }
    }
}