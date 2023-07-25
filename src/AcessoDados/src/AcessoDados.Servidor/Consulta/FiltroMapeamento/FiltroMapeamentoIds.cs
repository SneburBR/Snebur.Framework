using Snebur.AcessoDados.Estrutura;
using System.Collections.Generic;
using System.Linq;

namespace Snebur.AcessoDados.Mapeamento
{
    internal class FiltroMapeamentoIds : BaseFiltroMapeamento
    {

        internal SortedSet<long> Ids { get; }

        internal FiltroMapeamentoIds(SortedSet<long> ids, string nomeTipoEntidade) :
                                      base(new FiltroMapeamentoEntre(new FiltroMapeamentoVazio(),
                                                                       ids.Min(),
                                                                       ids.Max(),
                                                                       nomeTipoEntidade))
        {
            this.Ids = ids;
        }

        internal FiltroMapeamentoIds(EstruturaCampo estruturaCampoFiltro,
                                     SortedSet<long> ids) :
                                          base(new FiltroMapeamentoEntre(new FiltroMapeamentoVazio(),
                                                                         estruturaCampoFiltro,
                                                                         ids.Min(),
                                                                         ids.Max()))
        {
            this.EstruturaCampoFiltro = estruturaCampoFiltro;
            this.Ids = ids;
        }
    }
}