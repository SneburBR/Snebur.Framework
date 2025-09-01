
using Snebur.Dominio;
using System.Drawing;

namespace Snebur.Imagens;

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

