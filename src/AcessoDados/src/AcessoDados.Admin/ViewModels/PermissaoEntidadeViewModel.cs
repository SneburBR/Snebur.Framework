using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Utilidade;

namespace Snebur.AcessoDados.Admin.ViewModels
{
    public class PermissaoEntidadeViewModel : BasePermissaoViewModel
    {

        public string NomeTipoEntidade { get; }

        public IIdentificacao Identificacao { get; }

        public IPermissaoEntidade PermissaoEntidade { get; private set; }

        public ObservableCollection<PermissaoEntidadeViewModel> PermissoesEspecializadas { get; }

        public Dictionary<string, PermissaoCampoViewModel> PermissoesCampos { get; }

        public Type TipoEntidade { get; }

        public bool PermissaoTodosCamposCompleta
        {
            get
            {
                return (this.PermissoesCampos.Values.All(x => x.PermissaoCampo == null || x.PermissaoAtiva));
            }
        }

        public override bool ExisteInstanciaEntidade
        {
            get
            {
                return this.PermissaoEntidade != null;
            }
        }

        public bool PermissaoTodosCampos
        {
            get
            {
                return (this.PermissoesCampos.Values.All(x => x.PermissaoCampo == null || x.PermissaoCompleta));
            }
        }

        public PermissaoEntidadeViewModel PermissaoEntidadePai { get; set; }


        public PermissaoEntidadeViewModel(Type tipoEntidade, IIdentificacao identificacao,
                                          Dictionary<string, IPermissaoEntidade> permissoes,
                                          PermissaoEntidadeViewModel permissaoEntidadePai = null)
        {
            this.TipoEntidade = tipoEntidade;
            this.NomeTipoEntidade = this.TipoEntidade.Name;
            this.Identificacao = identificacao;
            this.PermissaoEntidadePai = permissaoEntidadePai;
            this.PermissaoEntidade = this.RetornarPermissaoEntidade(permissoes);

            //this.Leitura = new RegraOperacaoViewModel(this.PermissaoEntidade.Leitura, nameof(this.Leitura), permissaoEntidadePai?.Leitura);
            //this.Adicionar = new RegraOperacaoViewModel(this.PermissaoEntidade.Adicionar, nameof(this.Adicionar), permissaoEntidadePai?.Adicionar);
            //this.Atualizar = new RegraOperacaoViewModel(this.PermissaoEntidade.Atualizar, nameof(this.Atualizar), permissaoEntidadePai?.Atualizar);
            //this.Excluir = new RegraOperacaoViewModel(this.PermissaoEntidade.Excluir, nameof(this.Excluir), permissaoEntidadePai?.Excluir);

            if (System.Diagnostics.Debugger.IsAttached)
            {
                if (this.PermissaoEntidade == null)
                {
                    this.PermissaoEntidade = this.RetornarNovaInstanciaPermissaoEntidade();

                }

            }

            this.Leitura = new RegraOperacaoEntidadeViewModel(this, this.PermissaoEntidade?.Leitura, nameof(this.Leitura));
            this.Adicionar = new RegraOperacaoEntidadeViewModel(this, this.PermissaoEntidade?.Adicionar, nameof(this.Adicionar));
            this.Atualizar = new RegraOperacaoEntidadeViewModel(this, this.PermissaoEntidade?.Atualizar, nameof(this.Atualizar));
            this.Excluir = new RegraOperacaoEntidadeViewModel(this, this.PermissaoEntidade?.Excluir, nameof(this.Excluir));

            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.Leitura.RegraOperacao.Autorizado = true;
            }

            this.Leitura.RegraAlterada += this.RegraOperacao_Alterada;
            this.Adicionar.RegraAlterada += this.RegraOperacao_Alterada;
            this.Atualizar.RegraAlterada += this.RegraOperacao_Alterada;
            this.Excluir.RegraAlterada += this.RegraOperacao_Alterada;
            this.PermissoesCampos = this.RetornarPermissoesCampo();
            this.PermissoesEspecializadas = this.RetornarPermissoesEspecializadas(permissoes);

        }

        protected override void RegraOperacao_Alterada(object sender, EventArgs e)
        {
            base.RegraOperacao_Alterada(sender, e);

            var regraOperacaoViewModel = (sender as BaseRegraOperacaoViewModel);
            var nomeOperacao = regraOperacaoViewModel.NomeOperacao;

            this.NotifcarAlteracoesCampo(nomeOperacao);
            this.NotificarPropriedadeAlterada(nameof(this.PermissaoTodosCamposCompleta));
            this.NotificarPropriedadeAlterada(nameof(this.PermissaoTodosCampos));

            if (this.PermissoesEspecializadas != null)
            {
                foreach (var permissaoEntidade in this.PermissoesEspecializadas)
                {
                    permissaoEntidade.RegraOperacao_Alterada(sender, e);
                }

            }


        }

        private void NotifcarAlteracoesCampo(string nomeOperacao)
        {
            if (this.PermissoesCampos != null)
            {
                foreach (var campo in this.PermissoesCampos.Values)
                {
                    campo.NotificarPropriedadeAlteradaPublico(nomeOperacao);
                }
            }

        }


        private IPermissaoEntidade RetornarPermissaoEntidade(Dictionary<string, IPermissaoEntidade> permisseoes)
        {
            if (permisseoes.TryGetValue(this.NomeTipoEntidade, out IPermissaoEntidade permissaoEntidade))
            {
                return permissaoEntidade;
            }

            if (this.PermissaoEntidadePai == null)
            {
                return this.RetornarNovaInstanciaPermissaoEntidade();
            }
            return null;

        }

        private Dictionary<string, PermissaoCampoViewModel> RetornarPermissoesCampo()
        {
            var dicionario = new Dictionary<string, PermissaoCampoViewModel>();

            var permissaoEntidade = this.PermissaoEntidade;
            var tipoEntidade = this.TipoEntidade;

            var filtros = EnumFiltroPropriedadeCampo.IgnorarChavePrimaria |
                          EnumFiltroPropriedadeCampo.IgnorarPropriedadeProtegida |
                          EnumFiltroPropriedadeCampo.IgnorarTipoBase;


            var propriedadesCampo = EntidadeUtil.RetornarPropriedadesCampos(tipoEntidade, filtros).OrderBy(x => x.Name).ToList();

            var propriedadesRelacao = ReflexaoUtil.RetornarPropriedades(tipoEntidade, true).Where(x => x.GetCustomAttribute<ChaveEstrangeiraAttribute>() != null).ToList();

            var nomePropriedadesChaveEstrangeira = propriedadesRelacao.Select(x => x.GetCustomAttribute<ChaveEstrangeiraAttribute>().NomePropriedade).ToList();
            var propriedadesChaveEstrangeiras = propriedadesCampo.Where(x => nomePropriedadesChaveEstrangeira.Contains(x.Name)).ToList();

            propriedadesCampo = propriedadesCampo.Except(propriedadesChaveEstrangeiras).ToList();

            foreach (var propriedadeCampo in propriedadesCampo)
            {
                dicionario.Add(propriedadeCampo.Name, new PermissaoCampoViewModel(propriedadeCampo, null, this));
            }


            foreach (var propriedadeRelacao in propriedadesRelacao)
            {
                var nomePropriedadeChaveEstrangeira = propriedadeRelacao.GetCustomAttribute<ChaveEstrangeiraAttribute>().NomePropriedade;
                var propriedadeChaveEstrangeira = propriedadesChaveEstrangeiras.Where(x => x.Name == nomePropriedadeChaveEstrangeira).Single();

                dicionario.Add(propriedadeChaveEstrangeira.Name, new PermissaoCampoViewModel(propriedadeChaveEstrangeira, propriedadeRelacao, this));

            }
            return dicionario;
        }

        private ObservableCollection<PermissaoEntidadeViewModel> RetornarPermissoesEspecializadas(Dictionary<string, IPermissaoEntidade> permissoes)
        {
            var retorno = new ObservableCollection<PermissaoEntidadeViewModel>();
            var tiposEspecializada = this.RetornarTiposEspecializados();
            foreach (var tipoEntidadeEspecializada in tiposEspecializada)
            {
                retorno.Add(new PermissaoEntidadeViewModel(tipoEntidadeEspecializada, this.Identificacao, permissoes, this));
            }
            return retorno;
        }

        private List<Type> RetornarTiposEspecializados()
        {
            var tiposEspecializada = this.TipoEntidade.Assembly.GetTypes().Where(x => x.BaseType == this.TipoEntidade).ToList();
            return tiposEspecializada.Where(x => x.GetCustomAttribute<TabelaSegurancaAttribute>() == null).ToList();
        }

        public override string RetornarDescricao()
        {
            return this.TipoEntidade.Name;
        }

        public override bool RetornarPermissaoAtiva()
        {
            return this.Leitura.Autorizado ||
                   this.Adicionar.Autorizado ||
                   this.Atualizar.Autorizado ||
                   this.Excluir.Autorizado;
        }

        public override bool RetornarPermissaoCompleta()
        {
            if (this.PermissaoEntidade != null)
            {
                var resultado = this.PermissaoEntidade.Leitura.Autorizado &&
                                this.PermissaoEntidade.Adicionar.Autorizado &&
                                this.PermissaoEntidade.Atualizar.Autorizado &&
                                this.PermissaoEntidade.Excluir.Autorizado &&
                               !this.PermissaoEntidade.Leitura.AvalistaRequerido &&
                               !this.PermissaoEntidade.Adicionar.AvalistaRequerido &&
                               !this.PermissaoEntidade.Atualizar.AvalistaRequerido &&
                               !this.PermissaoEntidade.Excluir.AvalistaRequerido;

                if (resultado && this.PermissaoEntidade.PermissoesCampo.Count() > 0)
                {
                    resultado = this.PermissaoEntidade.PermissoesCampo.All(x => x.Atualizar.Autorizado &&
                                                                               !x.Atualizar.AvalistaRequerido &&
                                                                                x.Leitura.Autorizado &&
                                                                               !x.Leitura.AvalistaRequerido);
                }
                return resultado;
            }
            return false;

        }

        public void AtribuirNovaEntidadePermissaoEntidade()
        {
            if (this.PermissaoEntidade != null)
            {
                throw new InvalidOperationException($"a propriedade {nameof(this.PermissaoEntidade)} já está definida");
            }

            var novaInstancia = this.RetornarNovaInstanciaPermissaoEntidade();
            this.PermissaoEntidade = novaInstancia;
            this.NotificarPropriedadeAlterada(nameof(this.ExisteInstanciaEntidade));
        }

        private IPermissaoEntidade RetornarNovaInstanciaPermissaoEntidade()
        {
            var novaPermissaoEntidade = Activator.CreateInstance(Repositorio.TiposSeguranca.TipoPermissaoEntidade) as IPermissaoEntidade;
            novaPermissaoEntidade.NomeTipoEntidadePermissao = this.TipoEntidade.Name;
            novaPermissaoEntidade.Identificacao = this.Identificacao;
            novaPermissaoEntidade.Leitura = Activator.CreateInstance(Repositorio.TiposSeguranca.TipoRegraOperacao) as IRegraOperacao;
            novaPermissaoEntidade.Adicionar = Activator.CreateInstance(Repositorio.TiposSeguranca.TipoRegraOperacao) as IRegraOperacao;
            novaPermissaoEntidade.Atualizar = Activator.CreateInstance(Repositorio.TiposSeguranca.TipoRegraOperacao) as IRegraOperacao;
            novaPermissaoEntidade.Excluir = Activator.CreateInstance(Repositorio.TiposSeguranca.TipoRegraOperacao) as IRegraOperacao;
            return novaPermissaoEntidade;
        }

        public List<IPermissaoEntidade> RetornarTodosPermissoesEntidade()
        {
            var entidades = new List<IPermissaoEntidade>();

            if (this.PermissaoEntidade != null)
            {
                entidades.Add(this.PermissaoEntidade);
            }

            foreach (var permissaoEntidade in this.PermissoesEspecializadas.ToList())
            {
                entidades.AddRange(permissaoEntidade.RetornarTodosPermissoesEntidade());
            }
            return entidades;
        }
    }
}
