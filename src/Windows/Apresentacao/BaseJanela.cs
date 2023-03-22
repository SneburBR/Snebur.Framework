using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace Snebur.Windows
{
    public abstract class BaseJanela : Window, INotifyPropertyChanged
    {
        public BaseJanelaPrincipal JanelaPrincipal => (BaseJanelaPrincipal)Application.Current.MainWindow;

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void Ocupar()
        {
            this.JanelaPrincipal.Ocupar();
        }

        public virtual void Ocupar(string titulo, string mensagem)
        {
            this.JanelaPrincipal.Ocupar(titulo, mensagem);
        }

        public virtual void Desocupar()
        {
            this.JanelaPrincipal.Desocupar();
        }

        public virtual Task DesocuparAsync()
        {
            return this.JanelaPrincipal.DesocuparAsync();
        }

        public void NotificarPropriedadeAlterada<T>(T valor, [CallerMemberName] string nomePropriedade = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nomePropriedade));
        }
        public void NotificarPropriedadeAlterada([CallerMemberName] string nomePropriedade = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nomePropriedade));
        }

    }
}
