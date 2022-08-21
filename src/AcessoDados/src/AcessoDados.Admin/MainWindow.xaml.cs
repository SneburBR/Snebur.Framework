using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Snebur.AcessoDados.Admin.ViewModels;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Seguranca;
using Snebur.Utilidade;

namespace Snebur.AcessoDados.Admin
{
    public partial class MainWindow : Window
    {

        #region Propriedades dependentes

        public static readonly DependencyProperty IdentidadeSelecionadaProperty = DependencyProperty.Register("IdentidadeSelecionada",
                                                                                                    typeof(IdentificacaoViewModel),
                                                                                                    typeof(MainWindow),
                                                                                                    new PropertyMetadata(new PropertyChangedCallback(MainWindow.IdentidadeSelecionada_Alterada)));


        public static readonly DependencyProperty PermissaoEntidadeSelecionadaProperty = DependencyProperty.Register("PermissaoEntidadeSelecionada",
                                                                                                     typeof(PermissaoEntidadeViewModel),
                                                                                                     typeof(MainWindow),
                                                                                                     new PropertyMetadata(new PropertyChangedCallback(MainWindow.PermissaoEntidadeSelecionada_Alterada)));


        public static readonly DependencyProperty PermissaoCampoSelecionadoProperty = DependencyProperty.Register("PermissaoCampoSelecionado",
                                                                                                     typeof(PermissaoCampoViewModel),
                                                                                                     typeof(MainWindow),
                                                                                                     new PropertyMetadata(new PropertyChangedCallback(MainWindow.PermissaoCampoSelecionado_Alterado)));

        public static readonly DependencyProperty PermissaoSelecionadaProperty = DependencyProperty.Register("PermissaoSelecionada",
                                                                                                    typeof(BasePermissaoViewModel),
                                                                                                    typeof(MainWindow));
        //new PropertyMetadata(new PropertyChangedCallback(MainWindow.PermissaoSelecionada_Alterado)));

        public IdentificacaoViewModel IdentidadeSelecionada
        {
            get
            {
                return (IdentificacaoViewModel)this.GetValue(MainWindow.IdentidadeSelecionadaProperty);
            }
            set
            {
                this.SetValue(MainWindow.IdentidadeSelecionadaProperty, value);
            }
        }

        public PermissaoEntidadeViewModel PermissaoEntidadeSelecionada
        {
            get
            {
                return (PermissaoEntidadeViewModel)this.GetValue(MainWindow.PermissaoEntidadeSelecionadaProperty);
            }
            set
            {
                this.SetValue(MainWindow.PermissaoEntidadeSelecionadaProperty, value);
            }
        }

        public PermissaoCampoViewModel PermissaoCampoSelecionado
        {
            get
            {
                return (PermissaoCampoViewModel)this.GetValue(MainWindow.PermissaoCampoSelecionadoProperty);
            }
            set
            {
                this.SetValue(MainWindow.PermissaoCampoSelecionadoProperty, value);
            }
        }

        public BasePermissaoViewModel PermissaoSelecionada
        {
            get
            {
                return (BasePermissaoViewModel)this.GetValue(MainWindow.PermissaoSelecionadaProperty);
            }
            set
            {
                this.SetValue(MainWindow.PermissaoSelecionadaProperty, value);
            }
        }

        #endregion

        public ObservableCollection<PermissaoEntidadeViewModel> Permissoes { get; set; } = new ObservableCollection<PermissaoEntidadeViewModel>();
        public ObservableCollection<IdentificacaoViewModel> Identidades { get; set; } = new ObservableCollection<IdentificacaoViewModel>();
        public ObservableCollection<PermissaoCampoViewModel> Campos { get; set; } = new ObservableCollection<PermissaoCampoViewModel>();


        public MainWindow()
        {
            InitializeComponent();


            this.DataContext = this;
            this.Loaded += this.MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

                this.Inicializar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Environment.Exit(0);
            }
        }

        private void Inicializar()
        {
            using (var contexto = ContextoDadosUtil.RetornarContextoDados())
            {
                Repositorio.TiposSeguranca = contexto.TiposSeguranca;

                if (!Repositorio.TiposSeguranca.AtivarSeguranca)
                {
                    throw new Exception("A seguranção não pode ser ativado por os as classes não forem implementadas no contexto dados");
                }
            }

            this.PreencherIdentidades();
            this.PreencherPermissoes();
        }

        private static void IdentidadeSelecionada_Alterada(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MainWindow).PreencherPermissoes();

        }

        private static void PermissaoEntidadeSelecionada_Alterada(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MainWindow).PermissaoSelecionada = e.NewValue as BasePermissaoViewModel;
            (d as MainWindow).PreecherCampos();
        }

        private static void PermissaoCampoSelecionado_Alterado(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MainWindow).PermissaoSelecionada = e.NewValue as BasePermissaoViewModel;
        }

        private void PreencherIdentidades()
        {
            this.Identidades.Clear();
            using (var contexto = ContextoDadosUtil.RetornarContextoDados())
            {
                this.Identidades.Add(new IdentificacaoViewModel("Selecione uma identidade"));
                this.Identidades.Add(new IdentificacaoViewModel(contexto.RetornarUsuarioAnonimo()));

                var gruposUsuario = contexto.RetornarConsulta<IGrupoUsuario>(Repositorio.TiposSeguranca.TipoGrupoUsuario).OrderBy(x => x.Nome).ToList();
                foreach (var grupoUsuario in gruposUsuario)
                {
                    this.Identidades.Add(new IdentificacaoViewModel(grupoUsuario));
                }
            }
            this.IdentidadeSelecionada = this.Identidades.First();
        }

        private void PreencherPermissoes()
        {
            this.Permissoes.Clear();




            var identidadeSelecionada = this.IdentidadeSelecionada?.Identificacao;
            if (identidadeSelecionada != null)
            {
                var permissoes = this.RetornarDicionarioPermissoesEntidade(identidadeSelecionada);

                using (var contexto = ContextoDadosUtil.RetornarContextoDados())
                {

                    var tiposEntidade = contexto.GetType().GetProperties().
                                           Where(x => x.PropertyType.IsGenericType &&
                                                      x.PropertyType.GetGenericTypeDefinition() == typeof(IConsultaEntidade<>)).
                                                      Select(x => x.PropertyType.GetGenericArguments().Single()).ToList();

                    tiposEntidade = tiposEntidade.Where(x => x.GetCustomAttribute<TabelaSegurancaAttribute>() == null
                                                          && x.GetInterface(typeof(IIdentificacao).Name, false) == null
                                                          && x.GetInterface(typeof(ISessaoUsuario).Name, false) == null
                                                          && x.GetInterface(typeof(IIPInformacaoEntidade).Name, false) == null
                                                          && x.GetInterface(typeof(IAtividadeUsuario).Name, false) == null).ToList();

                    //tiposEntidade = ReflexaoUtil.IgnorarTiposComInterfaces(tiposEntidade, typeof(IIdentidade), typeof(ISessaoUsuario), typeof(IIPInformacaoEntidade), typeof(IAtividadeUsuario));

                    foreach (var tipoEntidade in tiposEntidade)
                    {
                        this.Permissoes.Add(new PermissaoEntidadeViewModel(tipoEntidade, identidadeSelecionada, permissoes));
                    }
                }

            }
        }

        private void PreecherCampos()
        {
            this.Campos.Clear();

            var permissaoEntidadeViewModel = this.PermissaoEntidadeSelecionada;
            if (permissaoEntidadeViewModel != null)
            {
                foreach (var permissaoCampo in permissaoEntidadeViewModel.PermissoesCampos.Values)
                {
                    this.Campos.Add(permissaoCampo);
                }
            }
        }

        public Dictionary<string, IPermissaoEntidade> RetornarDicionarioPermissoesEntidade(IIdentificacao identificacao)
        {
            var dicionarioPermissaoEntidades = new Dictionary<string, IPermissaoEntidade>();

            //using (var contexto = new ContextoMonitor())
            //{
            //    var consulta = contexto.PermissoesEntidade.Where(x => x.Identidade.Id == identidade.Id).
            //        AbrirRelacoes(x => x.Leitura, x => x.Adicionar, x => x.Atualizar, x => x.Excluir).
            //        AbrirColecao(x => x.PermissoesCampo).
            //        AbrirColecao(x => x.RestricoesFiltro).
            //        AbrirRelacoes(x => x.PermissoesCampo.Incluir().Leitura, x => x.PermissoesCampo.Incluir().Atualizar);

            //    var resultado = consulta.ToList();
            //}


            using (var contexto = ContextoDadosUtil.RetornarContextoDados())
            {
                //var relacoesAberta = ReflexaoUtil.RetornarNomesPropriedade<IPermissaoEntidade>(x => x.Adicionar,
                //                                                                                    x => x.Atualizar,
                //                                                                                    x => x.Leitura,
                //                                                                                    x => x.Excluir);

                //var colecoesAberta = ReflexaoUtil.RetornarNomesPropriedade<IPermissaoEntidade>(x => x.PermissoesCampo,
                //                                                                               x => x.RestricoesEntidade);


                //var expressoesReleacoesAbertaPermissaoCampo = ExpressaoUtil.RetornarExpressoes<IPermissaoEntidade>(
                //                                                        x => x.PermissoesCampo.Incluir().Leitura,
                //                                                        x => x.PermissoesCampo.Incluir().Atualizar);



                var consultaPermissoesEntidade = contexto.RetornarConsulta<IPermissaoEntidade>(contexto.TiposSeguranca.TipoPermissaoEntidade).
                                                                  Where(x => x.Identificacao.Id == identificacao.Id).
                                                                  AbrirRelacoes(x => x.Adicionar, x => x.Atualizar, x => x.Leitura, x => x.Excluir).
                                                                  AbrirColecao(x => x.PermissoesCampo).
                                                                  AbrirColecao(x => x.RestricoesEntidade).
                                                                  AbrirRelacao(x => x.PermissoesCampo.Incluir().Leitura).
                                                                  AbrirRelacao(x => x.PermissoesCampo.Incluir().Atualizar);


                var permissoesEntidade = consultaPermissoesEntidade.ToList();

                foreach (var permissoEntidade in permissoesEntidade)
                {
                    dicionarioPermissaoEntidades.Add(permissoEntidade.NomeTipoEntidadePermissao, permissoEntidade);
                }
            }

            return dicionarioPermissaoEntidades;
        }



        private void TreeViewPermissoes_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.PermissaoEntidadeSelecionada = (PermissaoEntidadeViewModel)this.TreeViewPermissoes.SelectedItem;
        }

        private void TreeViewCampos_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

            dynamic itemSelecionado = this.TreeViewCampos.SelectedItem;
            if (itemSelecionado is AtributoViewModel)
            {
                itemSelecionado = (itemSelecionado as AtributoViewModel).PermissaoCampoViewModel;
            }
            this.PermissaoCampoSelecionado = itemSelecionado as PermissaoCampoViewModel;
        }


        private void TreeViewPermissoes_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.PermissaoEntidadeSelecionada != null)
            {
                this.PermissaoSelecionada = PermissaoEntidadeSelecionada;
            }
        }

        private void ListViewCampos_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.PermissaoCampoSelecionado != null)
            {
                this.PermissaoSelecionada = PermissaoCampoSelecionado;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            var permissoesEntidadeViewModel = this.Permissoes.ToList();
            if (permissoesEntidadeViewModel.Count > 0)
            {
                var entidades = new List<IEntidade>();

                foreach (var pemissaoEntidadeViewmodel in permissoesEntidadeViewModel)
                {
                    entidades.AddRange(pemissaoEntidadeViewmodel.RetornarTodosPermissoesEntidade());
                }

                using (var contexto = ContextoDadosUtil.RetornarContextoDados())
                {
                    contexto.SalvarSeguranca(entidades);
                }
                MessageBox.Show(this, "Configuração salva com sucesso", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        #region ""

        private void TesteLeitura_Click(object sender, RoutedEventArgs e)
        {
            //using (var contexto = new ContextoMonitor())
            //{
            //    var logs = contexto.Logs.ToList();

            //}

        }

        private void TesteAtualizar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TesteAdicionar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TesteExcluir_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #region Menu 

        private void MenuGrupos_Click(object sender, RoutedEventArgs e)
        {
            var janela = new JanelaGrupos()
            {
                Owner = this
            };
            janela.ShowDialog();

        }

        private void MenuUsuarios_Click(object sender, RoutedEventArgs e)
        {
            var janela = new JanelaUsuarios()
            {
                Owner = this
            };
            janela.ShowDialog();

        }

        private void MenuTipoUsuarioAdicionar_Click(object sender, RoutedEventArgs e)
        {
            var janela = new JanelaTipoUsuarioAdicionarGrupoUsuario();
            janela.Owner = this;
            janela.ShowDialog();
        }

        #endregion


    }


}
