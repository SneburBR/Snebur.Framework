using System;
using Snebur.Dominio;

namespace Snebur.AcessoDados.Admin
{
    public class BaseEntidadeSegurancaViewModel : Snebur.Dominio.EntidadeViewModel
    {
        public BaseEntidadeSegurancaViewModel(Entidade entidade, Type tipo)
        {
            this.Entidade = entidade;
            if (this.Entidade == null)
            {
                this.Entidade = Activator.CreateInstance(tipo) as Entidade;
            }
        }
    }
}
