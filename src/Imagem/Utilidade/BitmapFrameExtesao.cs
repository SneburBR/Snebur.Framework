using System.Windows.Media.Imaging;

namespace Snebur.Imagens;

public static class BitmapFrameExtesao
{

    public static int RetornarRotacao(this BitmapFrame frame)
    {
        if (frame.Metadata is BitmapMetadata metadata)
        {
            var orientacaoString = metadata.GetQuery("/app1/ifd/{ushort=274}")?.ToString();
            if (!String.IsNullOrEmpty(orientacaoString) && Int32.TryParse(orientacaoString, out var orientacao) && orientacao > 0)
            {
                switch (orientacao)
                {
                    case 1:
                        return 0;
                    case 2:
                        //return RotateFlipType.RotateNoneFlipX;
                        return 0;
                    case 3:
                        //return RotateFlipType.Rotate180FlipNone;
                        return 180;
                    case 4:
                        //return RotateFlipType.Rotate180FlipX;
                        return 180;
                    case 5:
                        //return RotateFlipType.Rotate90FlipX;
                        return 90;
                    case 6:
                        //return RotateFlipType.Rotate90FlipNone;
                        return 90;
                    case 7:
                        //return RotateFlipType.Rotate270FlipX;
                        return 270;
                    case 8:
                        //return RotateFlipType.Rotate270FlipNone;
                        return 270;

                    default:

                        return 0;
                }
            }
        }
        return 0;
    }

    public static bool IsGirarFlipX(this BitmapFrame frame)
    {
        if (frame.Metadata is BitmapMetadata metadata)
        {
            var orientacaoString = metadata.GetQuery("/app1/ifd/{ushort=274}")?.ToString();
            if (!String.IsNullOrEmpty(orientacaoString) && Int32.TryParse(orientacaoString, out var orientacao) && 
                orientacao > 0)
            {
                return orientacao == 2 || orientacao == 4 || orientacao == 5 || orientacao == 7;
            }
        }
        return false;
    }
}
