using System;
using System.Collections.Generic;
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

namespace Snebur.AcessoDados.Admin
{
    /// <summary>
    /// Interaction logic for JanelaUsuarios.xaml
    /// </summary>
    public partial class JanelaUsuarios : BaseJanelaLista
    {
        public JanelaUsuarios()
        {
            InitializeComponent();
            this.Loaded += this.JanelaUsuarios_Loaded;
        }
       

        private void JanelaUsuarios_Loaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public override BaseJanelaCadastro RetornarJanelaCadastro(Entidade entidade)
        {
            throw new NotImplementedException();
        }

        public override IConsultaEntidade<IEntidade> RetornarConsulta(IContextoDadosSeguranca contexto)
        {
            return contexto.RetornarConsulta<IEntidade>(contexto.TiposSeguranca.TipoUsuario);
        }

        private void BtnNovo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnExcluir_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnEditarGrupos_Click(object sender, RoutedEventArgs e)
        {
            var entidade = this.EntidadeSelecionada;
            if (entidade != null)
            {
                var janela = new JanelaEditarGruposUsuario(entidade as IMembrosDe);
                janela.Owner = this;
                janela.ShowDialog();
            }
        }
    }
}
