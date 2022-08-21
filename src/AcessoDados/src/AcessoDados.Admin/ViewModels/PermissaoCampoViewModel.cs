using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Snebur.Dominio;
using Snebur.Utilidade;

namespace Snebur.AcessoDados.Admin.ViewModels
{
    public class PermissaoCampoViewModel : BasePermissaoViewModel
    {
        public IPermissaoCampo PermissaoCampo { get; set; }

        public PropertyInfo Propriedade { get; }

        public PermissaoEntidadeViewModel PermissaoEntidadeViewModel { get; }

        public IPermissaoEntidade PermissaoEntidade
        {
            get
            {
                return this.PermissaoEntidadeViewModel.PermissaoEntidade;
            }
        }

        public override bool ExisteInstanciaEntidade
        {
            get
            {
                return this.PermissaoCampo != null;
            }
        }

        public Type TipoPropriedade { get; }

        public string Nome { get; }

        public string NomeTipo { get; }

        public ObservableCollection<AtributoViewModel> Atributos { get; } = new ObservableCollection<AtributoViewModel>();

        public PermissaoCampoViewModel(PropertyInfo propriedade, PropertyInfo propriedadeRelacao, PermissaoEntidadeViewModel permissaoEntidadeViewModel)
        {
            this.Propriedade = propriedade;
            this.PermissaoEntidadeViewModel = permissaoEntidadeViewModel;
            this.TipoPropriedade = ReflexaoUtil.RetornarTipoSemNullable(this.Propriedade.PropertyType);
            this.NomeTipo = this.TipoPropriedade.Name;
            
            this.Nome = this.Propriedade.Name;
           

            if(propriedadeRelacao!= null)
            {
                this.Nome += $" ({propriedadeRelacao.PropertyType.Name})";
            }

            this.PermissaoCampo = this.RetornarPermissaoCampo();

            //this.Adicionar = null;
            //this.Excluir = new RegraOperacaoCampoViewModel(this);

            this.Leitura = new RegraOperacaoCampoViewModel(this,
                                                           this.PermissaoCampo?.Leitura,
                                                           nameof(this.Leitura),
                                                           (RegraOperacaoEntidadeViewModel)this.PermissaoEntidadeViewModel.Leitura);

            this.Atualizar = new RegraOperacaoCampoViewModel(this,
                                                             this.PermissaoCampo?.Atualizar,
                                                             nameof(this.Atualizar),
                                                             (RegraOperacaoEntidadeViewModel)this.PermissaoEntidadeViewModel.Atualizar);

            this.Leitura.RegraAlterada += this.RegraOperacao_Alterada;
            this.Atualizar.RegraAlterada += this.RegraOperacao_Alterada;
            this.PreencherAtributos();
        }

        

        private IPermissaoCampo RetornarPermissaoCampo()
        {

            var permissaoCampo = this.PermissaoEntidade?.PermissoesCampo.Where(x => x.NomeCampo == this.Propriedade.Name).SingleOrDefault();
            if (permissaoCampo != null)
            {
                return permissaoCampo;
            }
            return null;
            //permissaoCampo = Activator.CreateInstance(Repositorio.TiposSeguranca.TipoPermissaoCampo) as IPermissaoCampo;
            //permissaoCampo.Leitura = Activator.CreateInstance(Repositorio.TiposSeguranca.TipoRegraOperacao) as IRegraOperacao;
            //permissaoCampo.Atualizar = Activator.CreateInstance(Repositorio.TiposSeguranca.TipoRegraOperacao) as IRegraOperacao;
            //return permissaoCampo;
        }

        public override string RetornarDescricao()
        {
            return String.Format("{0} - {1}", this.PermissaoEntidadeViewModel.Descricao, this.Nome);
        }

        public override bool RetornarPermissaoAtiva()
        {
            return this.Leitura.Autorizado ||
                   this.Atualizar.Autorizado;
        }

        public override bool RetornarPermissaoCompleta()
        {
            if (this.PermissaoCampo != null)
            {
                return this.Leitura.Autorizado &&
                       this.Atualizar.Autorizado &&
                       !this.Leitura.AvalistaRequerido &&
                       !this.Atualizar.AvalistaRequerido;
            }
            return false;

        }

        public void AtribuirNovaEntidadePermissaoCampo()
        {
            if (this.PermissaoCampo != null)
            {
                throw new InvalidOperationException($"a propriedade {nameof(this.PermissaoCampo)} ja está definida");
            }

            if (this.PermissaoEntidade == null)
            {
                this.PermissaoEntidadeViewModel.AtribuirNovaEntidadePermissaoEntidade();
                throw new NotImplementedException();
            }

            var permissaoCampo = Activator.CreateInstance(Repositorio.TiposSeguranca.TipoPermissaoCampo) as IPermissaoCampo;
            permissaoCampo.NomeCampo = EntidadeUtil.RetornarNomeCampo(this.Propriedade);
            permissaoCampo.PermissaoEntidade = this.PermissaoEntidade;
            permissaoCampo.Leitura = Activator.CreateInstance(Repositorio.TiposSeguranca.TipoRegraOperacao) as IRegraOperacao;
            permissaoCampo.Atualizar = Activator.CreateInstance(Repositorio.TiposSeguranca.TipoRegraOperacao) as IRegraOperacao;

            this.PermissaoEntidade.PermissoesCampo.Add(permissaoCampo);
            this.PermissaoCampo = permissaoCampo;
            this.NotificarPropriedadeAlterada(nameof(this.ExisteInstanciaEntidade));



        }

        #region Atributos

        private void PreencherAtributos()
        {
            var atributos = this.Propriedade.GetCustomAttributes();
            foreach (var atributo in atributos)
            {
                this.Atributos.Add(new AtributoViewModel(this, atributo));
            }
        }
        #endregion

    }
}
