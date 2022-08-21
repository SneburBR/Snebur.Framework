using System;
using System.Threading.Tasks;
using System.Windows;
using Snebur.UI;

namespace Snebur.Windows
{
    public abstract class BaseJanelaPrincipal : BaseJanela
    {
        public JanelaOcupado JanelaOcupado { get; private set; }
        public JanelaMensagem JanelaMensagem { get; private set; }

        public BaseJanelaPrincipal()
        {
            this.Loaded += this.BaseJanelaPrincipal_Loaded;
        }

        private void BaseJanelaPrincipal_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public bool MostrarMensagem(string titulo, string mensagem, EnumBotoesAlerta botoesAlerta, string textoBotao = "")
        {
            if (this.JanelaMensagem == null)
            {
                this.IsEnabled = false;
                this.JanelaMensagem = new JanelaMensagem(titulo, mensagem, botoesAlerta, textoBotao);
                this.JanelaMensagem.Closed += this.JanelaMensagem_Closed;
                return (bool)this.JanelaMensagem.ShowDialog();
            }
            else
            {
                return false;
            }
        }

        private void JanelaMensagem_Closed(object sender, EventArgs e)
        {
            this.FecharMensagem();
        }

        private void FecharMensagem()
        {
            if (this.JanelaMensagem != null)
            {
                this.JanelaMensagem.Close();
                this.JanelaMensagem.Dispose();
                this.JanelaMensagem = null;
                this.IsEnabled = true;
            }
        }

        

        public override void Ocupar()
        {
            this.Ocupar(String.Empty, String.Empty);
        }

        public override void Ocupar(string titulo, string mensagem)
        {
            if (this.JanelaOcupado == null)
            {
                this.IsEnabled = false;
                this.JanelaOcupado = new JanelaOcupado(titulo, mensagem);
                this.JanelaOcupado.Closed += this.JanelaOcupado_Closed;
                this.JanelaOcupado.Show();
            }
        }

        private void JanelaOcupado_Closed(object sender, EventArgs e)
        {
            this.Desocupar();
        }

        public override Task DesocuparAwait()
        {
            return Task.Factory.StartNew(() =>
             {
                 this.Dispatcher.Invoke(this.Desocupar);
             });
        }

        public override void Desocupar()
        {
            if (this.JanelaOcupado != null)
            {
                this.JanelaOcupado.Close();
                this.JanelaOcupado.Dispose();
                this.JanelaOcupado = null;
                this.IsEnabled = true;
            }
        }
    }
}
