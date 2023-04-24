
namespace Snebur.Utilidade
{
    using SIGIFotografo;
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Snebur.Imagens;

    public class ImagemOrientacaoUtil
    {
        public static BitmapSource RetornarImagem(string caminhoArquivo)
        {
            return RetornarImagemInterno(caminhoArquivo, 0, 0, true);
        }


        public static BitmapSource RetornarImagemLarguraMaxima(string caminhoArquivo, double larguraMaxima)
        {
            return RetornarImagemDentro(caminhoArquivo, larguraMaxima, 0);
        }

        public static BitmapSource RetornarImagemAlturaMaxima(string caminhoArquivo, double alturaMaxima)
        {
            return RetornarImagemDentro(caminhoArquivo, 0, alturaMaxima);
        }

        public static BitmapSource RetornarImagemDentro(string caminhoArquivo, double larguraMaxima, double alturaMaxima)
        {

            return RetornarImagemInterno(caminhoArquivo, larguraMaxima, alturaMaxima, true);
        }

        public static BitmapSource RetornarImagemFora(string caminhoArquivo, double largura, double altura)
        {
            return RetornarImagemInterno(caminhoArquivo, largura, altura, false);
        }

        private static BitmapSource RetornarImagemInterno(string caminhoArquivo, double larguraDestino, double alturaDestino, bool isDentro)
        {

            using (var fs = new FileStream(caminhoArquivo, FileMode.Open, FileAccess.Read))
            {

                var decoder = BitmapDecoder.Create(fs, BitmapCreateOptions.None, BitmapCacheOption.None);
                var frame = decoder.Frames.First();
                var rotacao = frame.RetornarRotacao();
                BitmapSource bitmapSource = frame;

                if (rotacao != 0)
                {
                    bitmapSource = new TransformedBitmap(bitmapSource, new RotateTransform(rotacao));
                }
                var isGirarFlipX = frame.IsGirarFlipX();
                if (isGirarFlipX)
                {
                    bitmapSource = new TransformedBitmap(bitmapSource, new ScaleTransform(-1, 1));
                }

                if ((larguraDestino > 0 && bitmapSource.PixelHeight > larguraDestino) ||
                    (alturaDestino > 0 && bitmapSource.PixelHeight > alturaDestino))
                {

                    var scale = RetornarScalar(bitmapSource, larguraDestino, alturaDestino, isDentro);
                    bitmapSource = new TransformedBitmap(bitmapSource, new ScaleTransform(scale, scale));
                }

                var cache = new CachedBitmap(bitmapSource, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                cache.Freeze();
                return cache;
            }


        }

        private static double RetornarScalar(BitmapSource frame, double larguraMaxima, double alturaMaxima, bool isDentro)
        {
            if (larguraMaxima > 0 && alturaMaxima > 0)
            {
                var scaleX = larguraMaxima / (double)frame.PixelWidth;
                var scaleY = alturaMaxima / (double)frame.PixelHeight;

                if (isDentro)
                {
                    return Math.Min(scaleX, scaleY);
                }
                return Math.Max(scaleX, scaleY);

            }
            if (larguraMaxima > 0)
            {
                return larguraMaxima / (double)frame.PixelWidth;
            }

            if (alturaMaxima > 0)
            {
                return alturaMaxima / (double)frame.PixelHeight;
            }
            throw new ArgumentOutOfRangeException($"Ambos parametros não podem ser 0 {nameof(larguraMaxima)} {nameof(alturaMaxima)} ");
        }

        public static bool SalvarOrientacaoSeNecessario(string caminhoOrigem,
                                                        string caminhoDestino)
        {
            using (var fs = new FileStream(caminhoOrigem, FileMode.Open, FileAccess.Read))
            {

                var decoder = BitmapDecoder.Create(fs, BitmapCreateOptions.None, BitmapCacheOption.None);
                var frame = decoder.Frames.First();
                var rotacao = frame.RetornarRotacao();
                BitmapSource bitmapSource = frame;
                bool isNecessarioSalvar = false;

                if (rotacao != 0)
                {
                    bitmapSource = new TransformedBitmap(bitmapSource, new RotateTransform(rotacao));
                    isNecessarioSalvar = true;
                }

                var isGirarFlipX = frame.IsGirarFlipX();
                if (isGirarFlipX)
                {
                    bitmapSource = new TransformedBitmap(bitmapSource, new ScaleTransform(-1, 1));
                    isNecessarioSalvar = true;
                }

                if (isNecessarioSalvar)
                {
                    ArquivoUtil.DeletarArquivo(caminhoDestino);
                    var encoder = RetornarEncoder(decoder);

                    using (var fsDestino = new FileStream(caminhoDestino, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                        encoder.Save(fsDestino);
                        return true;
                    }
                }
            }
            return false;
        }

        private static BitmapEncoder RetornarEncoder(BitmapDecoder decoder)
        {
            switch (decoder)
            {
                //case GifBitmapDecoder gif:
                //    return new GifBitmapEncoder();
                case BmpBitmapDecoder bmp:
                    return new BmpBitmapEncoder();

                case PngBitmapDecoder png:

                    return new BmpBitmapEncoder();

                case TiffBitmapDecoder tif:

                    return new TiffBitmapEncoder();

                case JpegBitmapDecoder jpeg:
                default:

                    return new JpegBitmapEncoder
                    {
                        QualityLevel = 100
                    };
            }
        }
    }
}

namespace SIGIFotografo
{
    using System;
    using System.Drawing;
    using System.Windows.Media.Imaging;

    public static class BitmapFrameExtesao
    {

        public static RotateFlipType RetornarRotateFlipType(this BitmapFrame frame)
        {
            if (frame.Metadata is BitmapMetadata metadata)
            {
                var orientacaoString = metadata.GetQuery("/app1/ifd/{ushort=274}")?.ToString();
                if (!String.IsNullOrEmpty(orientacaoString) && Int32.TryParse(orientacaoString, out var orientacao) && orientacao > 0)
                {
                    switch (orientacao)
                    {
                        case 1:
                            return RotateFlipType.RotateNoneFlipNone;
                        case 2:
                            return RotateFlipType.RotateNoneFlipX;
                        case 3:
                            return RotateFlipType.Rotate180FlipNone;
                        case 4:
                            return RotateFlipType.Rotate180FlipX;
                        case 5:
                            return RotateFlipType.Rotate90FlipX;
                        case 6:
                            return RotateFlipType.Rotate90FlipNone;
                        case 7:
                            return RotateFlipType.Rotate270FlipX;
                        case 8:
                            return RotateFlipType.Rotate270FlipNone;
                        default:
                            return RotateFlipType.RotateNoneFlipNone;
                    }
                }
            }
            return RotateFlipType.RotateNoneFlipNone;
        }

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
            var rotateFlipType = frame.RetornarRotateFlipType();
            switch (rotateFlipType)
            {
                case RotateFlipType.RotateNoneFlipX:
                case RotateFlipType.Rotate180FlipX:
                case RotateFlipType.Rotate90FlipX:
                case RotateFlipType.Rotate270FlipX:
                    return true;
                default:
                    return false;
            }
        }
    }

}
