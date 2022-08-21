using System.Windows;
using System.Windows.Input;

namespace Snebur.Windows
{
    public partial class JanelaOcupado : BaseJanela
    {
        public string Titulo { get; set; }
        public string Mensagem { get; set; }

        public JanelaOcupado(string titulo, string mensagem)
        {
            this.Titulo = titulo;
            this.Mensagem = mensagem;

            this.InitializeComponent();

            this.Owner = this.JanelaPrincipal;
            this.Width = this.JanelaPrincipal.Width;
            this.Height = this.JanelaPrincipal.Height;
            this.Left = this.JanelaPrincipal.Left;
            this.Top = this.JanelaPrincipal.Top;
        }

        public new void Show()
        {
            base.Show();
            this.Cursor = Cursors.Wait;
        }

        public new void Close()
        {
            this.Cursor = Cursors.Arrow;
            base.Close();
        }

        private void BtnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
