using System;

namespace Snebur.RegrasNegocio
{
    public class BaseNegocio
    {

    }

    public class BaseNegocio<TContextoDados> : BaseNegocio
    {
        public TContextoDados ContextoDados { get; }

        public BaseNegocio(TContextoDados contexto)
        {
            this.ContextoDados = contexto;

            if (this.ContextoDados == null)
            {
                throw new Erro("O contexto dados não foi definido");
            }
        }
    }
}
