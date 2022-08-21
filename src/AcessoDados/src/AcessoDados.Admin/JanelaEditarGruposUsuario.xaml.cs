using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Snebur.Dominio;
using Snebur.Utilidade;

namespace Snebur.AcessoDados.Admin
{
    /// <summary>
    /// Interaction logic for JanelaEditarGruposTipoAdicionarGrupoUsuario.xaml
    /// </summary>
    public partial class JanelaEditarGruposUsuario : BaseJanela
    {
        public static readonly DependencyProperty GrupoSelecionadoProperty = DependencyProperty.Register("GrupoSelecionado",
                                                                                                    typeof(IGrupoUsuario),
                                                                                                    typeof(JanelaEditarGruposUsuario));

        public static readonly DependencyProperty MembroDeSelecionadoProperty = DependencyProperty.Register("MembroDeSelecionado",
                                                                                                    typeof(IGrupoUsuario),
                                                                                                    typeof(JanelaEditarGruposUsuario));

        public ObservableCollection<IGrupoUsuario> Grupos { get; } = new ObservableCollection<IGrupoUsuario>();

        public ObservableCollection<IGrupoUsuario> MembrosDe { get; } = new ObservableCollection<IGrupoUsuario>();

        public IMembrosDe Entidade { get; private set; }

        public IGrupoUsuario GrupoSelecionado
        {
            get
            {
                return (IGrupoUsuario)this.GetValue(JanelaEditarGruposUsuario.GrupoSelecionadoProperty);
            }
            set
            {
                this.SetValue(JanelaEditarGruposUsuario.GrupoSelecionadoProperty, value);
            }
        }

        public IGrupoUsuario MembroDeSelecionado
        {
            get
            {
                return (IGrupoUsuario)this.GetValue(JanelaEditarGruposUsuario.MembroDeSelecionadoProperty);
            }
            set
            {
                this.SetValue(JanelaEditarGruposUsuario.MembroDeSelecionadoProperty, value);
            }
        }

        public JanelaEditarGruposUsuario(IMembrosDe entidade)
        {
            InitializeComponent();

            this.DataContext = this;
            this.Entidade = entidade;
            this.Loaded += this.Janela_Loaded;

        }

        private void Janela_Loaded(object sender, RoutedEventArgs e)
        {
            using (var contexto = ContextoDadosUtil.RetornarContextoDados())
            {
                var consultaEntidade = contexto.RetornarConsulta<IMembrosDe>(this.Entidade.GetType()).Where(x => x.Id == this.Entidade.Id);
                consultaEntidade.AbrirColecao(ReflexaoUtil.RetornarCaminhoPropriedade<IMembrosDe>(x => x.MembrosDe));

                this.Entidade = consultaEntidade.SingleOrDefault();

                foreach (var grupoMembro in this.Entidade.MembrosDe)
                {
                    this.MembrosDe.Add(grupoMembro);
                }

                var grupos = contexto.RetornarConsulta<IGrupoUsuario>(contexto.TiposSeguranca.TipoGrupoUsuario).ToList();
                foreach (var grupo in grupos)
                {
                    if (!this.Entidade.MembrosDe.Contains(grupo) && !grupo.Equals(this.Entidade))
                    {
                        this.Grupos.Add(grupo);
                    }
                }
            }
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var grupoSelecionado = this.GrupoSelecionado;
            if (grupoSelecionado != null)
            {
                this.Entidade.MembrosDe.Add(grupoSelecionado);
                using (var contexto = ContextoDadosUtil.RetornarContextoDados())
                {
                    contexto.SalvarSeguranca(this.Entidade);
                }
                this.Grupos.Remove(grupoSelecionado);
                this.MembrosDe.Add(grupoSelecionado);
            }
        }

        private void ListView_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = !(this.GrupoSelecionado != null);

        }

      
        private void ListViewMembrosDe_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var membroDeSelecionado = this.MembroDeSelecionado;
            if (membroDeSelecionado != null)
            {
                this.Entidade.MembrosDe.Remove(membroDeSelecionado);
                using (var contexto = ContextoDadosUtil.RetornarContextoDados())
                {
                    contexto.SalvarSeguranca(this.Entidade);
                }
                this.MembrosDe.Remove(membroDeSelecionado);
                this.Grupos.Add(membroDeSelecionado);
            }
        }

        private void ListViewMembrosDe_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = !(this.MembroDeSelecionado != null);
        }
    }
}
