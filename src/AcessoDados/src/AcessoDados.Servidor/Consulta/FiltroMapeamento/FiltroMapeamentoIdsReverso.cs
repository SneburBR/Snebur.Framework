using Snebur.AcessoDados.Estrutura;
using System.Collections.Generic;

namespace Snebur.AcessoDados.Mapeamento
{
    internal class FiltroMapeamentoReverso : BaseFiltroMapeamento
    {
        internal SortedSet<long> Ids { get; }

        internal EstruturaCampo EstruturaCampoChavaEstrangeiraReversao { get; }

        internal FiltroMapeamentoReverso(EstruturaCampo estruturaCampoChavaEstrangeiraReversao,
                                           SortedSet<long> ids) : base(new FiltroMapeamentoVazio())
        {
            this.Ids = ids;
            this.EstruturaCampoFiltro = estruturaCampoChavaEstrangeiraReversao;
        }
    }
}
