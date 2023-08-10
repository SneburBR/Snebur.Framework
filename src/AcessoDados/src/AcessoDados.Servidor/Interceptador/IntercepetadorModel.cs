using System;

namespace Snebur.AcessoDados
{
    public class IntercepetadorModel
    {
        public Type TipoEntidade { get; }
        public Type[] TiposInterceptador { get; }
        public IInterceptador Instancia { get; }

        public IntercepetadorModel(Type tipoEntidade, 
                                   Type[] types)
        {
            this.TipoEntidade = tipoEntidade;
            this.TiposInterceptador = types;
            this.Instancia = (IInterceptador)Activator.CreateInstance(this.TiposInterceptador[0]);
            
            for(var i = 1; i < this.TiposInterceptador.Length; i++)
            {
                var intaciaBase = (IInterceptador)Activator.CreateInstance(this.TiposInterceptador[i]);
                this.Instancia.SetInterceptadorBase(intaciaBase);

            }
        }
    }
}
