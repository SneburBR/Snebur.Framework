using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Snebur.Windows;

public abstract class BaseControle : UserControl, INotifyPropertyChanged
{
    public BaseJanelaPrincipal JanelaPrincipal => (BaseJanelaPrincipal)Application.Current.MainWindow;

    public event PropertyChangedEventHandler? PropertyChanged;

    public void NotificarPropriedadeAlterada<T>(T valor, [CallerMemberName] string nomePropriedade = "")
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nomePropriedade));
    }
    public void NotificarPropriedadeAlterada([CallerMemberName] string nomePropriedade = "")
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nomePropriedade));
    }

}
