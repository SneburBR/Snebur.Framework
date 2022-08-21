using System.Reflection;
using System.Windows;

namespace Snebur
{
    public static class WindowExtensao
    {
        public static bool IsModal(this Window window)
        {
            return (bool)typeof(Window).GetField("_showingAsDialog", BindingFlags.Instance | 
                                                                     BindingFlags.NonPublic).GetValue(window);
        }
    }
}
