using Snebur.Utilidade;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Media;

namespace Snebur.Imagens
{
    public class CorFormatoUtil
    {
        private const int PixelFormatIndexed = 0x00010000;
        private const int PixelFormat32bppCMYK = 0x200F;
        private const int PixelFormat16bppGrayScale = (4 | (16 << 8));

        public static EnumFormatoCor RetornarFormatoCor(string caminhoArquivo)
        {
           
            using (var fs = new FileStream(caminhoArquivo, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return CorFormatoUtil.RetornarFormatoCor(fs);
            }
        }

        public static EnumFormatoCor RetornarFormatoCor(Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            try
            {
                if (SistemaUtil.IsWindowsXp)
                {
                    using (var bitmap = new Bitmap(stream))
                    {
                        return CorFormatoUtil.RetornarFormatoCor(bitmap);
                    }
                }
                else
                {
                    var decoder = DecoderUtil.RetornarDecoder(stream);
                    var formato = decoder.Frames.First().Format;
                    return RetornarFormatoCor(formato);
                }

            }
            catch (Exception)
            {
                return EnumFormatoCor.Desconhecido;
            }
        }

        public static EnumFormatoCor RetornarFormatoCor(Bitmap bitmap)
        {
            // Check image flags
            var flags = (ImageFlags)bitmap.Flags;
            if (flags.HasFlag(ImageFlags.ColorSpaceCmyk) || flags.HasFlag(ImageFlags.ColorSpaceYcck))
            {
                return EnumFormatoCor.Cmyk;
            }
            else if (flags.HasFlag(ImageFlags.ColorSpaceGray))
            {
                return EnumFormatoCor.Grayscale;
            }

            // Check pixel format
            var pixelFormat = (int)bitmap.PixelFormat;
            if (pixelFormat == PixelFormat32bppCMYK)
            {
                return EnumFormatoCor.Cmyk;
            }
            else if ((pixelFormat & PixelFormatIndexed) != 0)
            {
                return EnumFormatoCor.Indexed;
            }
            else if (pixelFormat == PixelFormat16bppGrayScale)
            {
                return EnumFormatoCor.Grayscale;
            }
            // Default to RGB
            return EnumFormatoCor.Rgb;
        }

        public static EnumFormatoCor RetornarFormatoCor(System.Windows.Media.PixelFormat formato)
        {
            if ((formato == PixelFormats.Cmyk32))

            {
                return EnumFormatoCor.Cmyk;
            }

            if ((formato == PixelFormats.Gray16) ||
                (formato == PixelFormats.Gray2) ||
                (formato == PixelFormats.Gray32Float) ||
                (formato == PixelFormats.Gray8) ||
                (formato == PixelFormats.BlackWhite) ||
                (formato == PixelFormats.Gray4))

            {
                return EnumFormatoCor.Grayscale;
            }

            if ((formato == PixelFormats.Indexed1) ||
                (formato == PixelFormats.Indexed2) ||
                (formato == PixelFormats.Indexed8))
            {
                return EnumFormatoCor.Indexed;
            }

            return EnumFormatoCor.Rgb;
        }
    }
}
