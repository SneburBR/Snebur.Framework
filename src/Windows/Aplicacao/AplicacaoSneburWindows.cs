using Snebur.Dominio;
using System.Windows;
using System.Windows.Threading;

namespace Snebur.Windows
{
    public class AplicacaoSneburWindows : AplicacaoSnebur
    {
        public override EnumTipoAplicacao TipoAplicacao => EnumTipoAplicacao.DotNet_Wpf;

        protected override dynamic DispatcherObject => this.Dispatcher;
        public Dispatcher Dispatcher => Application.Current.Dispatcher;
        public override bool IsMainThread => this.Dispatcher.CheckAccess();

        public AplicacaoSneburWindows() : base()
        {

        }

        
    }
}
