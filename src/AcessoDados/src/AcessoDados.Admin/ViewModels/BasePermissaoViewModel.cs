using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur.Dominio;

namespace Snebur.AcessoDados.Admin.ViewModels
{
    public abstract class BasePermissaoViewModel : BaseViewModel
    {
        /// <summary>
        /// Se existe registro do banco, ou configurações são atribuidas pela hirarquia
        /// </summary>
        public abstract bool ExisteInstanciaEntidade { get; }

        public bool PermissaoAtiva
        {
            get
            {
                return this.RetornarPermissaoAtiva();
            }
        }

        public bool PermissaoCompleta
        {
            get
            {
                return this.RetornarPermissaoCompleta();
            }
        }

        public string Descricao
        {
            get
            {
                return this.RetornarDescricao();
            }
        }

        public BaseRegraOperacaoViewModel Leitura { get; set; }

        public BaseRegraOperacaoViewModel Atualizar { get; set; }

        public BaseRegraOperacaoViewModel Adicionar { get; set; }

        public BaseRegraOperacaoViewModel Excluir { get; set; }

        public BasePermissaoViewModel()
        {
        }

        protected virtual void RegraOperacao_Alterada(object sender, EventArgs e)
        {
            this.NotificarPropriedadeAlterada(nameof(this.PermissaoAtiva));
            this.NotificarPropriedadeAlterada(nameof(this.PermissaoCompleta));

            var regraOperacaoViewModel = (sender as BaseRegraOperacaoViewModel);
            this.NotificarPropriedadeAlterada(regraOperacaoViewModel.NomeOperacao);
        }

        public void NotificarPropriedadeAlteradaPublico(string nomePropriedade)
        {
            this.NotificarPropriedadeAlterada(nomePropriedade);
        }


        public void NotificarPropriedadesPermissaoAtivaPermissaoCompleta()
        {
            this.NotificarPropriedadeAlterada(nameof(this.PermissaoAtiva));
            this.NotificarPropriedadeAlterada(nameof(this.PermissaoCompleta));
        }
        

        public abstract string RetornarDescricao();

        public abstract bool RetornarPermissaoAtiva();

        public abstract bool RetornarPermissaoCompleta();
    }
}
