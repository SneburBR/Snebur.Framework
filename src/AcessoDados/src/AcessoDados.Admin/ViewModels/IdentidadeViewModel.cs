using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur.Dominio;


namespace Snebur.AcessoDados.Admin.ViewModels
{
    public class IdentificacaoViewModel
    {
        public string Nome { get; }

        public IIdentificacao Identificacao { get; }

        public IdentificacaoViewModel(string nome)
        {
            this.Nome = nome;
        }
        public IdentificacaoViewModel(IUsuario usuario)
        {
            this.Identificacao = usuario;
            this.Nome = usuario.Nome;
        }

        public IdentificacaoViewModel(IGrupoUsuario grupoUsuario)
        {
            this.Identificacao = grupoUsuario;
            this.Nome = grupoUsuario.Nome;
        }

    }
}
