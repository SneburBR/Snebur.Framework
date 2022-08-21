using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows
{
   public static class UIElementExtensao
    {
        public static void Clear(this UIElement elemento)
        {
            elemento.Measure(new Size(1, 1));
            elemento.Arrange(new Rect(new Size(1, 1)));
            elemento.UpdateLayout();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
