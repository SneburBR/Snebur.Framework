

namespace Snebur 
{
    using System.Windows.Media;
    using Snebur.Dominio;

    public static partial class CorExtensao
    {
        public static Color RetornarCorWindows(this Cor cor)
        {
            return Color.FromRgb(cor.Red, cor.Green, cor.Blue);
        }

        public static SolidColorBrush RetornarBrushWindows(this Cor cor)
        {
            return new SolidColorBrush(cor.RetornarCorWindows());
        }
    }
}
 