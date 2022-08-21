using System.Windows;
using Snebur.Dominio;

namespace Snebur.AcessoDados.Admin
{
    /// <summary>
    /// Interaction logic for JanelaCadastroGrupoUsuario.xaml
    /// </summary>
    public partial class JanelaCadastroGrupoUsuario : BaseJanelaCadastro
    {
        public GrupoUsuarioViewModel ViewModel
        {
            get
            {
                return this.DataContext as GrupoUsuarioViewModel;
            }
        }


        public JanelaCadastroGrupoUsuario() : this(null)
        {

        }

        public JanelaCadastroGrupoUsuario(Entidade entidade)
        {
            InitializeComponent();
            this.DataContext = new GrupoUsuarioViewModel(entidade);
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (this.Salvar(this.ViewModel.Entidade))
            {
                this.DialogResult = true;
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
