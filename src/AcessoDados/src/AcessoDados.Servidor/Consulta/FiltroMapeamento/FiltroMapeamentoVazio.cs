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
    internal class FiltroMapeamentoVazio : BaseFiltroMapeamento
    {
        public FiltroMapeamentoVazio():base(null)
        {
        }
    }
}