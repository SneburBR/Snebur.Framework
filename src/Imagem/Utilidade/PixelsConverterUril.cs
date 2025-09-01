namespace Snebur.Imagens;

public class PixelsConverterUtil
{

    public static byte[] RgbaParaBgra(byte[] pixelsOrigemRgba, int largura, int altura)
    {
        var pixelsDestinoBgra = new byte[pixelsOrigemRgba.Length];
        var stride = largura * 4;
        for (var linha = 0; linha < altura; linha++)
        {
            for (var coluna = 0; coluna < largura; coluna++)
            {
                var posicao = (stride * linha) + (4 * coluna);
                byte red = pixelsOrigemRgba[posicao];
                byte green = pixelsOrigemRgba[posicao + 1];
                byte blue = pixelsOrigemRgba[posicao + 2];
                byte alpha = pixelsOrigemRgba[posicao + 3];

                pixelsDestinoBgra[posicao] = blue;
                pixelsDestinoBgra[posicao + 1] = green;
                pixelsDestinoBgra[posicao + 2] = red;
                pixelsDestinoBgra[posicao + 3] = alpha;
            }
        }
        return pixelsDestinoBgra;
    }

    public static byte[] BgraParaRgba(byte[] pixelsOrigemBgra, int largura, int altura)
    {
        var pixelsDestinoRgba = new byte[pixelsOrigemBgra.Length];
        var stride = largura * 4;
        for (var linha = 0; linha < altura; linha++)
        {
            for (var coluna = 0; coluna < largura; coluna++)
            {
                var posicao = (stride * linha) + (4 * coluna);

                byte blue = pixelsOrigemBgra[posicao];
                byte green = pixelsOrigemBgra[posicao + 1];
                byte red = pixelsOrigemBgra[posicao + 2];
                byte alpha = pixelsOrigemBgra[posicao + 3];

                pixelsDestinoRgba[posicao] = red;
                pixelsDestinoRgba[posicao + 1] = green;
                pixelsDestinoRgba[posicao + 2] = blue;
                pixelsDestinoRgba[posicao + 3] = alpha;
            }
        }
        return pixelsDestinoRgba;
    }
}
