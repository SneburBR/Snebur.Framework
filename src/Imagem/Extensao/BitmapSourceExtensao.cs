using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using Snebur.Imagem;
using Snebur.Utilidade;

namespace System.Windows.Media.Imaging
{
    public static class BitmapSourceExtensao
    {
        public static byte[] GetPixels(this BitmapSource imagem, EnumPixelFormato formato)
        {
            return RetornarPixels(imagem, formato);
        }

        public static byte[] RetornarPixels(this BitmapSource imagem)
        {
            return RetornarPixels(imagem, EnumPixelFormato.Bgra32);

        }
        public static byte[] RetornarPixels(this BitmapSource imagem, EnumPixelFormato formato)
        {
            var stride = imagem.PixelWidth * 4;
            var len = stride * imagem.PixelHeight;


            BitmapSource imagemBgra32;

            if (!imagem.IsBgraFormatoSuprotado())
            {
                imagemBgra32 = new FormatConvertedBitmap(imagem, PixelFormats.Bgra32, null, 0);
            }
            else
            {
                imagemBgra32 = imagem;
            }

            var pixelsBgra32 = new byte[len];
            imagemBgra32.CopyPixels(pixelsBgra32, stride, 0);

            switch (formato)
            {
                case EnumPixelFormato.Rgba:

                    var pixelsPixelsRgba = PixelsConverterUtil.BgraParaRgba(pixelsBgra32, imagem.PixelWidth, imagem.PixelHeight);
                    return pixelsPixelsRgba;

                case EnumPixelFormato.Bgra32:

                    return pixelsBgra32;

                default:

                    throw new Exception($"O formato não é suprotado {formato.ToString()}");
            }
        }

        public static bool IsBgraFormatoSuprotado(this BitmapSource imagem)
        {
            PixelFormat[] formatosBgrSuporteado = { PixelFormats.Bgra32, PixelFormats.Pbgra32 };
            return formatosBgrSuporteado.Contains(imagem.Format);
        }

        //public static void Dispose(this BitmapSource source)
        //{


        //}

        //public static PixelFormat RetornarPixelFormat(this BitmapSource source, string caminhoArquivo)
        //{
        //    if (SistemaUtil.IsWindowsXp)
        //    {
        //        using(var fs = StreamUtil.OpenRead(caminhoArquivo))
        //        {
        //            return source.RetornarPixelFormat(fs);
        //        }
        //    }
        //    return source.Format;
        //}

        //public static PixelFormat RetornarPixelFormat(this BitmapSource source, Stream streamOrigem)
        //{
        //    if (SistemaUtil.IsWindowsXp)
        //    {
        //        var corFormato = CorFormatoUtil.RetornarFormatoCor(streamOrigem);
        //        if(corFormato == EnumFormatoCor.Cmyk)
        //        {
        //            return PixelFormats.Cmyk32;
        //        }
        //        return source.Format;
        //    }
        //    return source.Format;
        //}

    }

    public enum EnumPixelFormato
    {
        //Formato para WEB - nativo do canvas
        Rgba = 1,
        //Formato para .NET -
        Bgra32 = 2
    }
}
