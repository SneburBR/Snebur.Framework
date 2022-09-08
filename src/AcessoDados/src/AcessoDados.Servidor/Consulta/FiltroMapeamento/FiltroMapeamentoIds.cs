using Snebur.AcessoDados.Estrutura;
using System.Collections.Generic;

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