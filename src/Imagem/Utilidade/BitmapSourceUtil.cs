using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Snebur.Imagens;

public class BitmapSourceUtil
{
    public static BitmapSource? AjustarDpi(BitmapSource origem, double dpi)
    {
        return AjustarDpi(origem, origem.Format, dpi);
    }

    public static BitmapSource? AjustarDpi(BitmapSource origem, PixelFormat formato, double dpi)
    {
        //var stride = origem.PixelWidth * 4;
        //var size = origem.PixelHeight * stride;
        //var pixels = new byte[size];
        //origem.CopyPixels(pixels, stride, 0);

        //var bmp = BitmapSource.Create(origem.PixelWidth, origem.PixelHeight, dpi, dpi,
        //                         origem.Format,
        //                         origem.Palette,
        //                         pixels,
        //                         stride);

        //Array.Resize(ref pixels, 0);
        //return bmp;
        return BitmapSourceUtil.CopiarImagem(origem, dpi);
    }

    public static BitmapSource? CopiarImagem(BitmapSource origem, double dpi = 0)
    {
        if (origem == null)
        {
            return null;
        }
        if (dpi == 0)
        {
            dpi = origem.DpiX;
        }

        var stride = origem.PixelWidth * 4;
        var size = origem.PixelHeight * stride;
        var pixels = new byte[size];
        origem.CopyPixels(pixels, stride, 0);

        var bmp = BitmapSource.Create(origem.PixelWidth, origem.PixelHeight, dpi, dpi,
                                      origem.Format,
                                      origem.Palette,
                                      pixels,
                                      stride);

        Array.Resize(ref pixels, 0);
        return bmp;

    }
}
