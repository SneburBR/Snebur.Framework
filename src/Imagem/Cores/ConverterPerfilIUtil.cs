//using ImageMagick;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using Snebur.Utilidade;

//namespace Snebur.Imagem
//{
//    public class ConverterPerfilIUtil
//    {
//        public const int QUALIDADE_PADRAO = 87;

//        #region 

//        public static void Converter(string caminhoOrigem, string caminhoDestino, EnumPerfilIcc perfil, int qualidade = QUALIDADE_PADRAO)
//        {
//            var nomePerfil = EnumUtil.RetornarNome(perfil);
//            ConverterPerfilIUtil.Converter(caminhoOrigem, caminhoDestino, nomePerfil, qualidade);
//        }

//        public static void Converter(string caminhoOrigem, string caminhoDestino, string nomePerfil, int qualidade = QUALIDADE_PADRAO)
//        {
//            var perfil = PerfilIccUtil.RetornarPerfilNativo(nomePerfil);
//            ConverterPerfilIUtil.ConverterNativo(caminhoOrigem, caminhoDestino, perfil, qualidade);
//        }

//        public static void Converter(Stream streamOrigem, Stream streamDestrino, string nomePerfil, int qualidade = ImagemUtil.QUALIDADE_PADRAO)
//        {
//            ConverterPerfilIUtil.Converter(streamOrigem, streamDestrino, nomePerfil, qualidade);
//        }

//        public static void ConverterNativo(Stream streamOrigem, MemoryStream streamDestrino, string nomePerfil, int qualidade = ImagemUtil.QUALIDADE_PADRAO)
//        {
//            var perfil = PerfilIccUtil.RetornarPerfilNativo(nomePerfil);
//            ConverterPerfilIUtil.ConverterNativo(streamOrigem, streamDestrino, perfil, qualidade);
//        }

//        #endregion

//        #region Converter MagickImage 

//        private static void ConverterMagickImage(string caminhoOrigem, string caminhoDestino, ColorProfile perfilDestino, int qualidade = QUALIDADE_PADRAO)
//        {

//            if (ArquivoUtil.CaminhoIgual(caminhoOrigem, caminhoDestino))
//            {
//                throw new Erro(String.Format("O caminho de origem deve ser diferente do destino. \n {0} \n {1}", caminhoOrigem, caminhoDestino));
//            }
//            ArquivoUtil.DeletarArquivo(caminhoDestino, false, true);
//            using (var msOrigem = RetornarStreamOrigem(caminhoOrigem))
//            {
//                using (var msDestino = ConverterPerfilIUtil.ConverterMagickImage(msOrigem, perfilDestino, qualidade))
//                {
//                    if (ImagemUtil.IsImagemJpeg(caminhoDestino))
//                    {
//                        File.WriteAllBytes(caminhoDestino, msDestino.ToArray());
//                    }
//                    else
//                    {
//                        var formato = ImagemUtil.RetornarFormatadoArquivo(caminhoOrigem);
//                        using (var msConvertido = ConverterImagemUtil.ConverterFormatado(caminhoOrigem, msDestino, formato, qualidade))
//                        {
//                            File.WriteAllBytes(caminhoDestino, msConvertido.ToArray());
//                        }
//                    }

//                }
//            }
//        }

//        private static MemoryStream ConverterMagickImage(string caminhoOrigem, ColorProfile perfilDestino, int qualidade = QUALIDADE_PADRAO)
//        {
//            using (var msOrigem = RetornarStreamOrigem(caminhoOrigem))
//            {
//                return ConverterPerfilIUtil.ConverterMagickImage(msOrigem, perfilDestino, qualidade);
//            }
//        }

//        private static MemoryStream ConverterMagickImage(Stream msOrigem, ColorProfile perfilDestino, int qualidade = QUALIDADE_PADRAO)
//        {
//            var msDestino = new MemoryStream();
//            ConverterPerfilIUtil.ConverterMagickImage(msOrigem, msDestino, perfilDestino, qualidade);
//            msDestino.Seek(0, SeekOrigin.Begin);
//            return msDestino;
//        }

//        private static void ConverterMagickImage(Stream streamOrigem, MemoryStream msDestino, ColorProfile perfilDestino, int qualidade = QUALIDADE_PADRAO)
//        {
//            using (var imagem = new MagickImage(streamOrigem))
//            {
//                if (imagem.ColorSpace == ColorSpace.Gray &&
//                    perfilDestino.ColorSpace != ColorSpace.Gray)
//                {
//                    using (var msOrigemCopia = new MemoryStream())
//                    {
//                        streamOrigem.Seek(0, SeekOrigin.Begin);
//                        streamOrigem.CopyTo(msDestino);

//                        imagem.Dispose();
//                        streamOrigem.Dispose();
//                        ConverterPerfilIUtil.ConverterGrayscale(msOrigemCopia, msDestino, perfilDestino);
//                    }
//                }
//                else
//                {

//                    if ((perfilDestino.Equals(imagem.GetColorProfile())) ||
//                        (PerfilIccUtil.IsPerfilsRGB(imagem.GetColorProfile()) &&
//                         PerfilIccUtil.IsPerfilsRGB(perfilDestino)))
//                    {
//                        streamOrigem.Seek(0, SeekOrigin.Begin);
//                        streamOrigem.CopyTo(msDestino);
//                        msDestino.Seek(0, SeekOrigin.Begin);
//                    }
//                    else
//                    {

//                        var perfilOrigem = ConverterPerfilIUtil.RetornarPerfilOrigem(imagem, perfilDestino);
//                        imagem.TransformColorSpace(perfilOrigem, perfilDestino);
//                        imagem.ColorSpace = perfilDestino.ColorSpace;

//                        imagem.RemoveProfile("icc");
//                        imagem.AddProfile(perfilDestino);

//                        //imagem.Format = MagickFormat.Jpeg;

//                        imagem.Quality = qualidade;
//                        imagem.Write(msDestino);

//                    }
//                }
//            }
//        }

//        internal static ColorProfile RetornarPerfilOrigem(MagickImage imagem, ColorProfile perfilDestino)
//        {
//            var perfil = imagem.GetColorProfile();
//            if (perfil == null)
//            {
//                if (imagem.ColorSpace == perfilDestino.ColorSpace)
//                {
//                    return perfilDestino;
//                }
//                else
//                {
//                    return PerfilIccUtil.RetornarPerfilPadrao(imagem.ColorSpace);
//                }
//            }
//            return perfil;
//        }

//        internal static MemoryStream RetornarStreamOrigem(string caminhoArquivo)
//        {
//            if (ImagemUtil.IsImagemJpeg(caminhoArquivo))
//            {
//                return new MemoryStream(File.ReadAllBytes(caminhoArquivo));
//            }
//            else
//            {
//                return ConverterImagemUtil.ConverterParaJpeg(caminhoArquivo);
//            }
//            //throw new NotImplementedException();
//        }

//        #region Grayscale

//        internal static void ConverterGrayscale(MemoryStream streamOrigem, MemoryStream streamDestino, ColorProfile perfilDestino)
//        {
//            using (var fsOrigem = ConverterPerfilIUtil.RetoranrStreamGrayscaleParaSGray(streamOrigem))
//            {
//                using (var msDestino_sRGB = new MemoryStream())
//                {
//                    var decoder = new JpegBitmapDecoder(fsOrigem, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);

//                    var img = decoder.Frames[0];
//                    var metadata = decoder.Frames[0].Metadata as BitmapMetadata;
//                    var thumbnail = decoder.Frames[0].Thumbnail;

//                    var perfilsRGB = PerfilIccUtil.sRGB;
//                    var contextos = new System.Collections.ObjectModel.ReadOnlyCollection<ColorContext>(new List<ColorContext>() { perfilsRGB });

//                    var convertImg = new FormatConvertedBitmap(img, PixelFormats.Rgb24, null, 0);
//                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();

//                    var frame = BitmapFrame.Create(convertImg, thumbnail, metadata, contextos);

//                    encoder.Frames.Add(frame);

//                    encoder.QualityLevel = 100;
//                    encoder.Save(msDestino_sRGB);

//                    //if (DebugUtil.IsAttached)
//                    //{
//                    //    var caminhoTemp = @"c:\temp\grayscale\grayscale_srgb_convertido.jpg";
//                    //    File.WriteAllBytes(caminhoTemp, msDestino_sRGB.ToArray());
//                    //}
//                    msDestino_sRGB.Seek(0, SeekOrigin.Begin);

//                    if (PerfilIccUtil.IsPerfilsRGB(perfilDestino))
//                    {
//                        streamDestino.Write(msDestino_sRGB.ToArray(), 0, (int)msDestino_sRGB.Length);
//                    }
//                    else
//                    {
//                        ConverterPerfilIUtil.ConverterMagickImage(msDestino_sRGB, streamDestino, perfilDestino);
//                    }


//                }
//            }
//        }

//        internal static MemoryStream RetoranrStreamGrayscaleParaSGray(MemoryStream streamOrigem)
//        {
//            var mssGray = new MemoryStream();
//            var bytessGray = File.ReadAllBytes(PerfilIccUtil.RetornarCaminhoPerfil_sGray());
//            var perfilSRgb = new ColorProfile(bytessGray);
//            ConverterPerfilIUtil.ConverterMagickImage(streamOrigem, mssGray, perfilSRgb);

//            //if (DebugUtil.IsAttached)
//            //{
//            //    var caminhoTemp = @"c:\temp\grayscale\sgray_convertido.jpg";
//            //    File.WriteAllBytes(caminhoTemp, mssGray.ToArray());
//            //}
//            mssGray.Seek(0, SeekOrigin.Begin);
//            return mssGray;
//        }

//        #endregion

//        #endregion

//        #region Converter Nativo .NET

//        public static void ConverterNativo(string caminhoOrigem, string caminhoDestino, ColorContext perfilDestino, int qualidade = QUALIDADE_PADRAO)
//        {
//            using (var fsOrigem = StreamUtil.OpenRead(caminhoOrigem))
//            {
//                using (var msDestino = new MemoryStream())
//                {
//                    ConverterPerfilIUtil.ConverterNativo(fsOrigem, msDestino, perfilDestino, qualidade);
//                    File.WriteAllBytes(caminhoDestino, msDestino.ToArray());
//                }
//            }
//        }

//        public static void ConverterNativo(Stream streamOrigem, Stream streamDestrino, ColorContext perfilDestino, int qualidade = ImagemUtil.QUALIDADE_PADRAO)
//        {

//            var decoder = BitmapDecoder.Create(streamOrigem, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
//            var frame = decoder.Frames.First();
//            var bitmapSource = ConverterNativo(frame, perfilDestino);

//            var colorContexts = perfilDestino.RetornarColorContexts();
//            var encoder = EncoderUtil.RetornarEncoder(decoder, qualidade);
//            var novoFrame = BitmapFrame.Create(bitmapSource as BitmapSource, frame.Thumbnail, frame.Metadata as BitmapMetadata, colorContexts);
//            novoFrame.Freeze();

//            encoder.Frames.Add(novoFrame);
//            encoder.Save(streamDestrino);

//            if (streamDestrino.CanSeek)
//            {
//                streamDestrino.Seek(0, SeekOrigin.Begin);
//            }

//            bitmapSource = null;
//            novoFrame = null;
//            frame = null;
//            decoder = null;
//            encoder = null;

//            GC.Collect();

//        }

//        public static BitmapSource ConverterNativo(Stream streamOrigem, ColorContext perfilDestino, int qualidade = ImagemUtil.QUALIDADE_PADRAO)
//        {
//            if (streamOrigem.CanSeek)
//            {
//                streamOrigem.Seek(0, SeekOrigin.Begin);
//            }
//            var decoder = BitmapDecoder.Create(streamOrigem, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
//            var frame = decoder.Frames.First();

//            var bitmapSourConvertido = ConverterPerfilIUtil.ConverterNativo(frame, perfilDestino);

//            decoder = null;
//            frame = null;

//            GC.Collect();

//            return bitmapSourConvertido;
//        }

//        private static object bloqueio = new object();

//        public static BitmapSource ConverterNativo(BitmapFrame frame, ColorContext perfilDestino)
//        {
//            lock (bloqueio)
//            {
//                try
//                {
//                    var bitmapSourceDestino = frame as BitmapSource;
//                    var formatoOrigem = CorFormatoUtil.RetornarFormatoCor(frame.Format);
//                    var perfilOrigem = frame.RetornarPerfilNativo();

//                    if (perfilOrigem != null && formatoOrigem == EnumFormatoCor.Rgb && 
//                        perfilOrigem.IsPerfilsRGB() && 
//                        perfilDestino.IsPerfilsRGB())
//                    {
//                        return frame;
//                    }

//                    if (formatoOrigem == EnumFormatoCor.Indexed)
//                    {
//                        var converterRgb = new FormatConvertedBitmap();
//                        converterRgb.BeginInit();
//                        converterRgb.Source = frame;
//                        converterRgb.DestinationFormat = PixelFormats.Pbgra32;
//                        converterRgb.EndInit();
//                        converterRgb.Freeze();

//                        bitmapSourceDestino = converterRgb;
//                        formatoOrigem = EnumFormatoCor.Rgb;
//                    }

//                    if (perfilOrigem == null ||
//                        perfilOrigem.RetornarCorFormato() != formatoOrigem)
//                    {
//                        if (!(perfilOrigem != null && formatoOrigem == EnumFormatoCor.Indexed &&
//                              perfilOrigem.RetornarCorFormato() == EnumFormatoCor.Rgb))
//                        {
//                            perfilOrigem = PerfilIccUtil.RetornarPerfilNativoPadrao(formatoOrigem);
//                        }
//                    }

//                    var bitmapConvertido = new ColorConvertedBitmap();
//                    bitmapConvertido.BeginInit();
//                    bitmapConvertido.SourceColorContext = perfilOrigem;
//                    bitmapConvertido.Source = bitmapSourceDestino;
//                    bitmapConvertido.DestinationFormat = RetornarFormatoDestino(perfilDestino);
//                    bitmapConvertido.DestinationColorContext = perfilDestino;
//                    bitmapConvertido.EndInit();
//                    bitmapConvertido.Freeze();

//                    //bitmapSourceDestino = null;
//                    //if (SistemaUtil.IsWindowsXp || DebugUtil.IsAttached)
//                    //{
//                    //    var bitmapImage = new BitmapImage();
//                    //    bitmapImage.BeginInit();
//                    //    bitmapImage.StreamSource = ImagemUtil.RetornarMemoryStream(bitmapConvertido, 100);
//                    //    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
//                    //    bitmapImage.EndInit();

//                    //    using (var ms = ImagemUtil.RetornarMemoryStream(bitmapImage, 100))
//                    //    {
//                    //    }

//                    //    bitmapConvertido = null;

//                    //    return bitmapImage;
//                    //}
//                    return bitmapConvertido;
//                }
//                catch (Exception ex)
//                {
//                    throw new Exception($"Não foi possivel converter a imagem do formato {frame.Format.ToString()} ", ex);
//                }
//            }

//        }

//        private static PixelFormat RetornarFormatoDestino(ColorContext perfil)
//        {
//            var corFormato = perfil.RetornarCorFormato();
//            switch (corFormato)
//            {
//                case EnumFormatoCor.Cmyk:

//                    return PixelFormats.Cmyk32;

//                case EnumFormatoCor.Grayscale:

//                    return PixelFormats.Gray16;

//                case EnumFormatoCor.Rgb:

//                    return PixelFormats.Pbgra32;

//                default:
//                    throw new Exception($"Cor formato não suportado {EnumUtil.RetornarDescricao(corFormato) } ");


//            }

//        }

//        #endregion

//    }

//}

////.NET NATIVO
////public static BitmapSource ConverterPerfil(BitmapSource imagem, ColorContext perfilOrigem, ColorContext perfilConverter)
////{


////    return bitmapConverter;
////}

