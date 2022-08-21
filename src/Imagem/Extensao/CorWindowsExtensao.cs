
namespace Snebur
{
    using System.Windows.Media;
    using Snebur.Dominio;

    public static class CorWindowsExtensao
    {
        public static Color RetornarCorWIndows(this Cor cor)
        {

            return Color.FromArgb(cor.Alpha, cor.Red, cor.Green, cor.Blue);
        }

        public static SolidColorBrush RetornarBrushWindows(this Cor cor)
        {
            return new SolidColorBrush(cor.RetornarCorWIndows());
        }
    }
}

