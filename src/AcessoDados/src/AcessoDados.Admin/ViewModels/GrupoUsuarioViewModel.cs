using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur.Dominio;

namespace Snebur.AcessoDados.Admin
{
    public class GrupoUsuarioViewModel : BaseEntidadeSegurancaViewModel
    {
        public IGrupoUsuario GrupoUsuario
        {
            get
            {
                return this.Entidade as IGrupoUsuario;
            }
        }

        public GrupoUsuarioViewModel(Entidade entidade) : base(entidade, Repositorio.TiposSeguranca.TipoGrupoUsuario)
        {

        }
    }
}
