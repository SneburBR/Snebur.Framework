using System;

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