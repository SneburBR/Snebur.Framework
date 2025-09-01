namespace Snebur.Imagens;

public class UnidadeUtil
{
    private double POLEGADA = 2.54;

    private double PixelToCm(int totalPixels, int dpi = 96)
    {
        return (totalPixels / 300) * POLEGADA;
    }

    private int CmToPixel(double centimetros, int dpi = 96)
    {
        return (int)Math.Round((centimetros / POLEGADA) * dpi);
    }
}
