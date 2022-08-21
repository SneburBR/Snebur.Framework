using System.Windows;
using System.Windows.Threading;

namespace Snebur.Windows
{
    public class AplicacaoSneburWindows : AplicacaoSnebur
    {
        protected override dynamic DispatcherObject => this.Dispatcher;
        public Dispatcher Dispatcher => Application.Current.Dispatcher;
        public override bool IsMainThread => this.Dispatcher.CheckAccess();

        public AplicacaoSneburWindows() : base()
        {

        }

    }
}
