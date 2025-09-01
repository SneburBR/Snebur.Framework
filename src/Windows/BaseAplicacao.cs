using System.Windows;
using System.Windows.Navigation;

namespace Snebur.Windows;

public abstract partial class BaseAplicacao : Application
{

    public BaseAplicacao()
    {
        //var caminhosEstilo = new List<string>
        //{
        //    "Estilos/Cores.xaml",
        //    "Estilos/padrao.xaml",
        //    "Estilos/Textos/TextBlock.xaml"
        //};

        //foreach(var caminhoEstilo in caminhosEstilo)
        //{
        //    var uriEstilo = new Uri($"/Snebur.Windows;component/{caminhoEstilo}", UriKind.Relative);
        //    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = uriEstilo });
        //}
    }
    protected override void OnStartup(StartupEventArgs e)
    {

    }

    protected override void OnLoadCompleted(NavigationEventArgs e)
    {
        base.OnLoadCompleted(e);

        var mw = Application.Current.MainWindow;
        var appSn = this.RetornarAplicacaoSnebur();
        appSn.InicializarAsync();
    }

    protected abstract AplicacaoSnebur RetornarAplicacaoSnebur();

}
