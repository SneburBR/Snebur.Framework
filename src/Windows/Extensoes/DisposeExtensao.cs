using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Snebur.Windows
{
    public static class DisposeExtensao
    {
        public static void Dispose(this FrameworkElement frameworkElement)
        {
            if (frameworkElement != null)
            {
                frameworkElement.Width = 1;
                frameworkElement.Height = 1;
                GC.SuppressFinalize(frameworkElement);
            }
            GC.WaitForPendingFinalizers();
        }

        public static void Dispose(this DispatcherObject dispatcher)
        {
            if (dispatcher != null)
            {
                GC.SuppressFinalize(dispatcher);
            }


            GC.WaitForPendingFinalizers();
        }
    }
}
