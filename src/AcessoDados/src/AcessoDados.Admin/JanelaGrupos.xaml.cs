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

namespace Snebur.AcessoDados.Admin
{
    public partial class JanelaGrupos : BaseJanelaLista
    {

        public JanelaGrupos() : base()
        {
            InitializeComponent();

        }

        private void BtnNovo_Click(object sender, RoutedEventArgs e)
        {
            this.Novo();
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            this.Editar();
        }

        private void BtnExcluir_Click(object sender, RoutedEventArgs e)
        {
            this.Excluir();
        }


        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public override BaseJanelaCadastro RetornarJanelaCadastro(Entidade entidade)
        {
            return new JanelaCadastroGrupoUsuario(entidade);
        }

        public override IConsultaEntidade<IEntidade> RetornarConsulta(IContextoDadosSeguranca contexto)
        {
            return contexto.RetornarConsulta<IEntidade>(contexto.TiposSeguranca.TipoGrupoUsuario) as IConsultaEntidade<IEntidade>;

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
