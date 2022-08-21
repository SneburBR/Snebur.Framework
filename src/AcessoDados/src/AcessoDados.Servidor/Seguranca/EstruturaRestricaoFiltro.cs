using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur.Dominio;

namespace Snebur.AcessoDados.Seguranca
{
    internal class EstruturaRestricaoFiltro
    {
        internal IRestricaoFiltroPropriedade RestricaoFiltro { get; set; }

        internal EstruturaRestricaoFiltro(IRestricaoFiltroPropriedade restricao)
        {
            this.RestricaoFiltro = restricao;
        }
    }
}
