using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Snebur.Utilidade;

namespace Snebur.Imagens
{
    public class ConverterPerfilNativo : ConverterPerfil
    {
        private bool ManterMetadata { get; set; } = true;
        private EnumFormatoCor CorFormato { get; }
        //private PixelFormat PixelFormat { get; set; }  

        public ConverterPerfilNativo(string caminhoOrigem, ColorContext perfilDestinoNativo, int qualidade, int dpi, bool alterarDpi, EnumFormatoCor corFormato = EnumFormatoCor.Desconhecido) :
                                    base(caminhoOrigem, perfilDestinoNativo, qualidade, dpi, alterarDpi)
        {
            this.CorFormato = corFormato;
        }

        public ConverterPerfilNativo(Stream streamOrigem, ColorContext perfilDestinoNativo, int qualidade, int dpi, bool alterarDpi, EnumFormatoCor corFormato = EnumFormatoCor.Desconhecido) :
                                     base(streamOrigem, perfilDestinoNativo, qualidade, dpi, alterarDpi)
        {
            this.CorFormato = corFormato;
        }

        public ConverterPerfilNativo(ColorContext perfilDestnoNativo) : base(perfilDestnoNativo)
        {

        }

        private static object bloqueio = new object();

        public BitmapSource RetornarBitmapSourceConvertido(BitmapFrame frame)
        {
            var perfilOrigem = frame.RetornarPerfilOrigem();
            return this.RetornarBitmapSourceConvertido(frame, perfilOrigem);
        }

        public BitmapSource RetornarBitmapSourceConvertido(BitmapSource bitmapSource, ColorContext perfilOrigem)
        {
            var formatoOrigem = CorFormatoUtil.RetornarFormatoCor(bitmapSource.Format);
            //var bitmapSourceDestino = frame as BitmapSource;
            var forcar = false;
            try
            {
                //var perfilOrigem = frame.RetornarPerfilNativo();
                if (formatoOrigem == EnumFormatoCor.Indexed &&
                    (perfilOrigem != null && perfilOrigem.RetornarCorFormato() == EnumFormatoCor.Rgb))
                {
                    var converterRgb = new FormatConvertedBitmap();
                    converterRgb.BeginInit();
                    converterRgb.Source = bitmapSource;
                    converterRgb.DestinationFormat = PixelFormats.Bgra32;
                    converterRgb.EndInit();
                    converterRgb.Freeze();

                    bitmapSource = converterRgb;
                    formatoOrigem = EnumFormatoCor.Rgb;

                    forcar = true;
                }

                if (perfilOrigem == null ||
                    perfilOrigem.RetornarCorFormato() != formatoOrigem)
                {
                    if (!(perfilOrigem != null && formatoOrigem == EnumFormatoCor.Indexed &&
                          perfilOrigem.RetornarCorFormato() == EnumFormatoCor.Rgb))
                    {
                        perfilOrigem = PerfilIccUtil.RetornarPerfilNativoPadrao(formatoOrigem);
                        forcar = true;
                    }
                }

                //if (SistemaUtil.IsWindowsXp)
                //{
                //    perfilOrigem = PerfilIccUtil.RetornarPerfilNativo(EnumPerfilIcc.sRGB);
                //}

                var pixalFormatDestino = ConverterPerfilNativo.RetornarFormatoDestinoPadrao(this.PerfilDestinoNativo);
                if (!perfilOrigem.Igual(this.PerfilDestinoNativo) || forcar)
                {
                    return this.RetornarBitmapSourceConvertidoInterno(bitmapSource, perfilOrigem, this.PerfilDestinoNativo, pixalFormatDestino);
                }
                return bitmapSource;

            }
            catch (Exception ex)
            {
                throw new Erro($"Não foi possivel converter a imagem do formato {formatoOrigem.ToString()} ", ex);
            }
            finally
            {
                bitmapSource = null;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                Thread.Sleep(100);
            }
        }


        public override void Salvar(string caminhoDestino)
        {
            ArquivoUtil.DeletarArquivo(caminhoDestino, true);
            using (var fs = StreamUtil.CreateWrite(caminhoDestino))
            {
                this.Salvar(fs);
            }

            if (!ImagemUtil.IsArquivoBinarioImagem(caminhoDestino))
            {
                throw new ErroImagemCorrompida($"Imagem comrrompida no converter (Nativo) Origem: {caminhoDestino}");
            }
        }

        public override void Salvar(Stream streamDestino)
        {
            try
            {
                this.SalvarInterno(streamDestino);
            }
            catch (Exception)
            {
                if (!this.ManterMetadata)
                {
                    throw;

                }
                this.ManterMetadata = false;
                this.Salvar(streamDestino);
            }
        }

        private void SalvarInterno(Stream streamDestino, bool clonarMetadata = false)
        {
            try
            {
                using (var ms = this.RetornarMemoryStreamConvertido(clonarMetadata))
                {
                    StreamUtil.SalvarStreamBufferizada(ms, streamDestino);
                }
            }
            catch (Exception)
            {
                if (clonarMetadata)
                {
                    throw;
                }

                StreamUtil.SetarPonteiroInicio(streamDestino);
                SalvarInterno(streamDestino, true);
            }
        }


        private MemoryStream RetornarMemoryStreamConvertido(bool clonarMetadata)
        {
            var msDestino = new MemoryStream();
            using (var streamOrigem = this.RetornarStreamOrigem())
            {

                BitmapDecoder decoder;

                if (SistemaUtil.IsWindowsXp && this.CorFormato == EnumFormatoCor.Cmyk)
                {
                    decoder = BitmapDecoder.Create(streamOrigem, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                }
                else
                {
                    decoder = DecoderUtil.RetornarDecoder(streamOrigem);
                }

                //var decoder = DecoderUtil.RetornarDecoder(streamOrigem);
                var frame = decoder.Frames.First();

                var bitmapSourceConvertido = this.RetornarBitmapSourceConvertido(frame);

                var colorContexts = this.PerfilDestinoNativo.RetornarColorContexts();

                var metadata = this.RetornarMetadata( frame, clonarMetadata);

                var encoder = EncoderUtil.RetornarEncoder(decoder, this.Qualidade);
                if (bitmapSourceConvertido.DpiX < this.Dpi || bitmapSourceConvertido.DpiY < this.Dpi)
                {
                    bitmapSourceConvertido = BitmapSourceUtil.AjustarDpi(bitmapSourceConvertido, Dpi);
                }
                var novoFrame = BitmapFrame.Create(bitmapSourceConvertido as BitmapSource, null, metadata, colorContexts);
                encoder.Frames.Add(novoFrame);

                try
                {
                    encoder.Save(msDestino);
                }
                catch (Exception)
                {
                    throw;
                }

                decoder = null;
                frame = null;
                bitmapSourceConvertido = null;
                colorContexts = null;
                encoder = null;
            }

            try
            {
                using (var streamTeste = StreamUtil.RetornarMemoryStream(msDestino))
                {
                    var decoder = DecoderUtil.RetornarDecoder(streamTeste);
                    var frame = decoder.Frames.First();
                    frame = null;
                    decoder = null;
                }

            }
            catch (Exception ex)
            {

                LogUtil.ErroAsync(new Exception($"Não foi possivel converter o arquivo {this.CaminhoArquivoOrigem}", ex));
                throw;

            }
            finally
            {
                GC.Collect();
            }
            //msDestino.Position = 0;
            return msDestino;
        }

        private BitmapMetadata RetornarMetadata( BitmapFrame frame, bool clonarMetadata)
        {
            if (SistemaUtil.IsWindowsXp)
            {
                return null;
            }

            if (this.ManterMetadata)
            {
                return MetadataUtil.RetornarMetadata(frame, clonarMetadata);
               
            }
            return null;
        }

        private ColorConvertedBitmap RetornarBitmapSourceConvertidoInterno(BitmapSource bitmap, ColorContext perfilOrigem, ColorContext perfilDestino, PixelFormat formatoDestino)
        {
            var bitmapConvertido = new ColorConvertedBitmap();
            bitmapConvertido.BeginInit();
            bitmapConvertido.SourceColorContext = perfilOrigem;
            bitmapConvertido.Source = bitmap;
            bitmapConvertido.DestinationFormat = formatoDestino;
            bitmapConvertido.DestinationColorContext = perfilDestino;
            bitmapConvertido.EndInit();
            bitmapConvertido.Freeze();
            return bitmapConvertido;
        }

        public static PixelFormat RetornarFormatoDestinoPadrao(ColorContext perfil)
        {
            //if (this.PixelFormat != null)
            //{
            //    return this.PixelFormat;
            //}

            var corFormato = perfil.RetornarCorFormato();
            switch (corFormato)
            {
                case EnumFormatoCor.Cmyk:

                    return PixelFormats.Cmyk32;

                case EnumFormatoCor.Grayscale:

                    return PixelFormats.Gray16;

                case EnumFormatoCor.Rgb:

                    return PixelFormats.Pbgra32;

                default:

                    throw new Exception($"Cor formato não suportado {EnumUtil.RetornarDescricao(corFormato) } ");
            }
        }

        private EnumFormatoCor RetornarCorFormatoOrigem(BitmapFrame frame)
        {
            if (SistemaUtil.IsWindowsXp)
            {
                using (var streamOrigem = base.RetornarStreamOrigem())
                {
                    return CorFormatoUtil.RetornarFormatoCor(streamOrigem);
                }
            }
            return CorFormatoUtil.RetornarFormatoCor(frame.Format);
        }

        //private static PixelFormat RetornarPixelFormataGrayscale(PixelFormat pixelFormatOrigem)
        //{
        //    if (pixelFormatOrigem == PixelFormats.Gray2 ||
        //        pixelFormatOrigem == PixelFormats.Gray4 ||
        //        pixelFormatOrigem == PixelFormats.Gray8 ||
        //        pixelFormatOrigem == PixelFormats.Gray16 ||
        //        pixelFormatOrigem == PixelFormats.Gray32Float)
        //    {
        //        return pixelFormatOrigem;
        //    }
        //    return PixelFormats.Gray16;
        //}

        //private static PixelFormat RetornarPixelFormataRgb(PixelFormat pixelFormatOrigem)
        //{
        //    if (pixelFormatOrigem == PixelFormats.Pbgra32 ||
        //        pixelFormatOrigem == PixelFormats.Prgba64 ||
        //        pixelFormatOrigem == PixelFormats.Prgba128Float ||
        //        pixelFormatOrigem == PixelFormats.Rgb24 ||
        //        pixelFormatOrigem == PixelFormats.Rgb48 ||
        //        pixelFormatOrigem == PixelFormats.Rgba64 ||
        //        pixelFormatOrigem == PixelFormats.Rgb128Float ||
        //        pixelFormatOrigem == PixelFormats.Bgr24 ||
        //        pixelFormatOrigem == PixelFormats.Bgr32 ||
        //        pixelFormatOrigem == PixelFormats.Bgr555 ||
        //        pixelFormatOrigem == PixelFormats.Bgr565 ||
        //        pixelFormatOrigem == PixelFormats.Bgra32 ||
        //        pixelFormatOrigem == PixelFormats.Bgr24)
        //    {
        //        return pixelFormatOrigem;
        //    }
        //    return PixelFormats.Bgr24;
        //}

        //private PixelFormat RetornarFormatoDestino(PixelFormat pixelFormatOrigem, ColorContext perfil)
        //{

        //    var corFormato = perfil.RetornarCorFormato();
        //    switch (corFormato)
        //    {
        //        case EnumFormatoCor.Cmyk:

        //            return PixelFormats.Cmyk32;

        //        case EnumFormatoCor.Grayscale:

        //            return RetornarPixelFormataGrayscale(pixelFormatOrigem);

        //        case EnumFormatoCor.Rgb:

        //            return RetornarPixelFormataRgb(pixelFormatOrigem);

        //        default:
        //            throw new Exception($"Cor formato não suportado {EnumUtil.RetornarDescricao(corFormato) } ");
        //    }
        //}
    }
}
