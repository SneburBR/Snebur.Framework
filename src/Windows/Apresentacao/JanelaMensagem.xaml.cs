using System;
using System.Windows;
using System.Windows.Input;
using Snebur.UI;

namespace Snebur.Windows
{
    /// <summary>
    /// Interaction logic for JanelaOcupado.xaml
    /// </summary>
    public partial class JanelaMensagem : BaseJanela, IJanelaAlerta
    {

        public string Titulo { get; set; }
        public string Mensagem { get; set; }
        public string TextoBotaoPersonalizado { get; set; }

        public EnumResultadoAlerta ResultadoAlerta { get; private set; }

        public JanelaMensagem(string titulo, string mensagem, EnumBotoesAlerta botoesAlerta, string textoBotaoPersonalizado = "")
        {

            this.DataContext = this;

            this.Titulo = titulo;
            this.Mensagem = mensagem;

            this.InitializeComponent();

            this.AtualizarExibicaoBotoes(botoesAlerta, textoBotaoPersonalizado);

            this.Owner = this.JanelaPrincipal;
            this.Width = this.JanelaPrincipal.Width;
            this.Height = this.JanelaPrincipal.Height;
            this.Left = this.JanelaPrincipal.Left;
            this.Top = this.JanelaPrincipal.Top;
        }

        private void AtualizarExibicaoBotoes(EnumBotoesAlerta btnMensagem, string TextoBotaoPersonalizado)
        {
            this.btnPersonalizado.Visibility = Visibility.Collapsed;
            this.btnOk.Visibility = Visibility.Collapsed;
            this.btnCancelar.Visibility = Visibility.Collapsed;
            this.btnSim.Visibility = Visibility.Collapsed;
            this.btnNao.Visibility = Visibility.Collapsed;
            this.btnFechar.Visibility = Visibility.Collapsed;
            this.btnVoltar.Visibility = Visibility.Collapsed;

            switch (btnMensagem)
            {
                case EnumBotoesAlerta.SimNao:
                    this.btnSim.Visibility = Visibility.Visible;
                    this.btnNao.Visibility = Visibility.Visible;
                    break;


                case EnumBotoesAlerta.Fechar:

                    this.btnFechar.Visibility = Visibility.Visible;

                    break;

                case EnumBotoesAlerta.FecharVoltar:

                    this.btnVoltar.Visibility = Visibility.Visible;
                    this.btnFechar.Visibility = Visibility.Visible;

                    break;

                case EnumBotoesAlerta.Nenhum:

                    break;

                case EnumBotoesAlerta.Ok:
                    this.btnOk.Visibility = Visibility.Visible;
                    break;

                case EnumBotoesAlerta.OkCancelar:
                    this.btnOk.Visibility = Visibility.Visible;
                    this.btnCancelar.Visibility = Visibility.Visible;
                    break;

                case EnumBotoesAlerta.Personalizado:
                    this.btnPersonalizado.Visibility = Visibility.Visible;
                    this.TextoBotaoPersonalizado = TextoBotaoPersonalizado;
                    break;

                default:
                    throw new Erro("Tipo da JanelaMensagem não implementada");
            }
            
        }

        public new bool? ShowDialog()
        {
            return base.ShowDialog();
        }

        public new void Show()
        {
            base.Show();
        }

        public new void Close()
        {
            base.Close();
        }
        
        private void BotaoSimOkFechar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void BotaoNaoCancelar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        public void Fechar()
        {
            this.Close();
        }

        public void Dispose()
        {
            
        }
    }
}
