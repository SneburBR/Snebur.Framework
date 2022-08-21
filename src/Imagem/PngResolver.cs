using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using Snebur.Utilidade;

namespace Snebur.Imagem
{
    public class PngResolver
    {
        public static bool UsarPng { get; set; } = true;

        public static BitmapSource RetornarImagem(Uri uri, int alturaMaxima = 0)
        {
            using (var ms = new MemoryStream(HttpUtil.RetornarBytes(uri.AbsoluteUri)))
            {
                if (!PngResolver.UsarPng)
                {
                    return PngResolver.RetornarBitmapSourcePngComoJpeg(ms, alturaMaxima);
                }
                else
                {
                    return PngResolver.RetornarImagem(ms, alturaMaxima);
                }
            }
        }

        public static BitmapSource RetornarImagem(string caminho, int alturaMaxima = 0)
        {
            using (var fs = StreamUtil.OpenRead(caminho))
            {
                if (!PngResolver.UsarPng)
                {
                    return PngResolver.RetornarBitmapSourcePngComoJpeg(fs, alturaMaxima);
                }
                else
                {
                    return PngResolver.RetornarImagem(fs, alturaMaxima);
                }
            }
        }

        public static BitmapSource RetornarImagem(Stream stream, int alturaMaxima = 0)
        {
            try
            {
                var bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = stream;
                if (alturaMaxima > 0)
                {
                    bi.DecodePixelHeight = alturaMaxima;
                }
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.EndInit();
                bi.Freeze();
                return bi;
            }
            catch
            {
                stream.Seek(0, SeekOrigin.Begin);
                PngResolver.UsarPng = false;
                return PngResolver.RetornarBitmapSourcePngComoJpeg(stream, alturaMaxima);
            }

        }

      

        public static BitmapSource RetornarBitmapSourcePngComoJpeg(Stream stream, int alturaMaxima = 0)
        {
            using (var msJpge = RetornarStreamComoJpeg(stream))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                if (alturaMaxima > 0)
                {
                    bitmap.DecodePixelHeight = alturaMaxima;
                }

                bitmap.StreamSource = msJpge;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;

                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
        }


        public static Stream RetornarStreamComoJpeg(Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            using (var imagemJpeg = System.Drawing.Image.FromStream(stream))
            {

                var msJpge = new MemoryStream();

                var jpgEncoder = BitmapUtil.RetornarEncoder(ImageFormat.Jpeg);
                var myEncoder = System.Drawing.Imaging.Encoder.Quality;
                var parametros = new EncoderParameters(1);
                var meuParametro = new EncoderParameter(myEncoder, 100L);
                parametros.Param[0] = meuParametro;

                imagemJpeg.Save(msJpge, jpgEncoder, parametros);
                msJpge.Seek(0, SeekOrigin.Begin);
                return msJpge;
            }

        }

        //public static BitmapImage RetornarLogo()
        //{
        //    if (!PngResolver.UsarPng)
        //    {
        //        return PngResolver.RetornarLogoJpeg();
        //    }

        //    try
        //    {
        //        using (var msLogo = new MemoryStream(File.ReadAllBytes(Configuracao.caminhoLogo)))
        //        {
        //            var bi = new BitmapImage();
        //            bi.BeginInit();
        //            bi.StreamSource = msLogo;
        //            bi.CacheOption = BitmapCacheOption.OnLoad;
        //            bi.EndInit();
        //            bi.Freeze();
        //            return bi;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return PngResolver.RetornarLogoJpeg();
        //    }
        //}

        //private static BitmapImage RetornarLogoJpeg()
        //{
        //    using (var msLogo = new MemoryStream(File.ReadAllBytes(Configuracao.caminhoLogo)))
        //    {
        //        using (var imagemJpeg = System.Drawing.Image.FromStream(msLogo))
        //        {

        //            using (var msJpge = new MemoryStream())
        //            {
        //                var jpgEncoder = BitmapUtil.RetornarEncoder(ImageFormat.Jpeg);
        //                var myEncoder = System.Drawing.Imaging.Encoder.Quality;
        //                var parametros = new EncoderParameters(1);
        //                var meuParametro = new EncoderParameter(myEncoder, 100L);
        //                parametros.Param[0] = meuParametro;

        //                imagemJpeg.Save(msJpge, jpgEncoder, parametros);

        //                var bitmap = new BitmapImage();
        //                bitmap.BeginInit();
        //                bitmap.StreamSource = new MemoryStream(msJpge.ToArray());
        //                bitmap.CacheOption = BitmapCacheOption.OnLoad;
        //                bitmap.EndInit();
        //                //bitmap.Freeze();
        //                return bitmap;
        //            }
        //        }
        //    }
        //}
    }
}