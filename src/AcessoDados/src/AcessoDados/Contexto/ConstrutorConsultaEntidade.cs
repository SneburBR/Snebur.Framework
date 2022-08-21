using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;

namespace Snebur.AcessoDados
{
    public abstract class ConstrutorConsultaEntidade
    {
        public __BaseContextoDados ContextoDados { get; set; }
        public Type TipoEntidade { get; set; }

        public ConstrutorConsultaEntidade(__BaseContextoDados contexto, Type tipoEntidade)
        {
            this.ContextoDados = contexto;
            this.TipoEntidade = tipoEntidade;
        }

         
    }
}