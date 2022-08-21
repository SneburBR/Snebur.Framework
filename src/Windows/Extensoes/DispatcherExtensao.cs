using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Snebur.Windows
{
    public static class DispatcherExtensao
    {
        public static DispatcherOperation BeginInvoke(this Dispatcher dispatcher, Action acao)
        {
            return dispatcher.BeginInvoke((Delegate)acao);
        }
    }
}
