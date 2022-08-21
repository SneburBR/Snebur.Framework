namespace Snebur
{
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using Snebur.Dominio;

    public static class CorDrawingExtensao
    {
        public static Color RetornarCorDrawing(this Cor cor)
        {

            return Color.FromArgb(cor.Alpha, cor.Red, cor.Green, cor.Blue);
        }

        public static SolidBrush RetornarBrushDrawing(this Cor cor)
        {
            return new SolidBrush(cor.RetornarCorDrawing());
        }
    }
}






