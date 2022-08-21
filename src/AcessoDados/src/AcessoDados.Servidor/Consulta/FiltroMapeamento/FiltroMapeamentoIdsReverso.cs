using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur.AcessoDados.Estrutura;

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
