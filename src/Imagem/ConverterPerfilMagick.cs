using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Snebur.Utilidade;

namespace Snebur.Imagem
{

    public class ConverterPerfilMagick : ConverterPerfil
    {
        private ColorProfile PerfilDestinoMagick { get; }

        public ConverterPerfilMagick(string caminhoOrigem, ColorContext perfilDestinoNativo, int qualidade, int dpi, bool alterarDpi) :
                                     base(caminhoOrigem, perfilDestinoNativo, qualidade, dpi, alterarDpi)
        {
            this.PerfilDestinoMagick = this.RetornarColorProfile(this.PerfilDestinoNativo);
        }

        public ConverterPerfilMagick(Stream streamOrigem, ColorContext perfilDestinoNativo, int qualidade, int dpi, bool alterarDpi) :
                                    base(streamOrigem, perfilDestinoNativo, qualidade, dpi, alterarDpi)
        {
            this.PerfilDestinoMagick = this.RetornarColorProfile(this.PerfilDestinoNativo);
        }

        private ColorProfile RetornarColorProfile(ColorContext colorContext)
        {
            using (var stream = colorContext.OpenProfileStream())
            {
                using (var ms = StreamUtil.RetornarMemoryStream(stream))
                {
                    return new ColorProfile(ms.ToArray());
                }
            }
        }

        private static object bloqueio = new object();

        public override void Salvar(string caminhoDestino)
        {
            using (var ms = new MemoryStream())
            {
                this.Salvar(ms);
                ArquivoUtil.DeletarArquivo(caminhoDestino);
                File.WriteAllBytes(caminhoDestino, ms.ToArray());
            }
        }

        public override void Salvar(Stream streamDestino)
        {
            this.SalvarInterno(streamDestino, 0);
        }

        private void SalvarInterno(Stream streamDestino, int tentativa)
        {
            lock (bloqueio)
            {
                try
                {
                    using (var msTemporaria = new MemoryStream())
                    {
                        using (var streamOrigem = this.RetornarStreamOrigem())
                        {
                            using (var imagem = new MagickImage(streamOrigem))
                            {
                                if (imagem.ColorSpace == ColorSpace.Gray ||
                                    (this.PerfilDestinoMagick.ColorSpace == ColorSpace.Gray))
                                {
                                    throw new Exception("Formato gray não suportado por Magick");
                                }

                                var perfilOrigem = this.RetornarPerfilOrigem(imagem, this.PerfilDestinoMagick);
                                imagem.ColorSpace = this.PerfilDestinoMagick.ColorSpace;
                                imagem.TransformColorSpace(perfilOrigem, this.PerfilDestinoMagick);
                                imagem.Format = MagickFormat.Jpeg;
                                imagem.Quality = this.Dpi;

                                var exif = imagem.GetProfile("exif");
                                var exifTipado = exif as ExifProfile;

                            
                               // this.AjustarDpi(imagem);

                                imagem.Write(msTemporaria);
                            }
                        }

                        if (this.AlterarDpi)
                        {
                            this.ConferirImagem(msTemporaria, streamDestino);
                        }
                        else
                        {
                            StreamUtil.SalvarStreamBufferizada(msTemporaria, streamDestino);
                        }
                        
                    }
                }
                catch
                {
                    this.SalvarNativo(streamDestino);
                }
                finally
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
        }

        //private void AjustarDpi(MagickImage imagem)
        //{
        //    var exif = imagem.GetProfile("exif") as ExifProfile;
        //    if (exif == null)
        //    {
        //        exif = new ExifProfile();
        //    }

        //    var dpix = Convert.ToInt32(exif.GetValue(ExifTag.XResolution));
        //    var dpiy = Convert.ToInt32(exif.GetValue(ExifTag.YResolution));
        //    var xxx = exif.GetValue(ExifTag.ResolutionUnit);

        //    if (dpix < this.Dpi || dpiy < this.Dpi)
        //    {
        //        exif = new ExifProfile();
        //        var dpaix = (decimal)550;

        //        exif.SetValue(ExifTag.XResolution, dpaix);
        //        exif.SetValue(ExifTag.XResolution, (double)550);
        //        //exif.SetValue(ExifTag.YResolution, this.Dpi);

        //        imagem.RemoveProfile("exit");
        //        imagem.AddProfile(exif);
        //    }
        //}

        private void ConferirImagem(MemoryStream msOrigem, Stream streamDestino)
        {
            using (var msConferencia = StreamUtil.RetornarMemoryStream(msOrigem))
            {
                BitmapDecoder decoder = null;
                try
                {
                    decoder = BitmapDecoder.Create(msConferencia, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                }
                catch
                {
                    throw new ErroImagemCorrompida($"Imagem comrrompida no converter (Magick) : Origem {this.CaminhoArquivoOrigem}");
                }

                var formatoValidacao = CorFormatoUtil.RetornarFormatoCor(decoder.Frames.First().Format);
                if (formatoValidacao == EnumFormatoCor.Grayscale && this.PerfilDestinoNativo.RetornarCorFormato() != EnumFormatoCor.Grayscale)
                {
                    throw new Exception("Formato gray não suportado por Magick");
                }
                var encoder = EncoderUtil.RetornarEncoder(decoder, this.Qualidade);
                var frame = decoder.Frames.First();
                var bitmapSouce = frame as BitmapSource;
                var metadata = MetadataUtil.RetornarMetadata(frame);

                if (frame.DpiX >= this.Dpi || frame.DpiY >= this.Dpi)
                {
                    StreamUtil.SalvarStreamBufferizada(msOrigem, streamDestino);
                    return;
                }
                else
                {
                    bitmapSouce = BitmapSourceUtil.AjustarDpi(bitmapSouce, this.Dpi);
                }
                var perfil = frame.RetornarPerfilNativo();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSouce, null, metadata, perfil.RetornarColorContexts()));

                using (var msConferido = new MemoryStream())
                {
                    encoder.Save(msConferido);
                    //File.WriteAllBytes(@"c:\temp\dsadasdsa.jpg", msConferido.ToArray());
                    StreamUtil.SalvarStreamBufferizada(msConferido, streamDestino);
                }

                frame = null;
                metadata = null;
                decoder = null;
                encoder = null;
            }
        }

        private void SalvarNativo(Stream streamDestino)
        {
            using (var streamOrigem = this.RetornarStreamOrigem())
            {
                using (var converter = new ConverterPerfilNativo(streamOrigem, this.PerfilDestinoNativo, 100, this.Dpi, this.AlterarDpi))
                {
                    converter.Salvar(streamDestino);
                }
            }
        }

        private ColorProfile RetornarPerfilOrigem(MagickImage imagem, ColorProfile perfilDestino)
        {
            var perfil = imagem.GetColorProfile();
            if (perfil == null)
            {
                return PerfilIccUtil.RetornarPerfilPadrao(imagem.ColorSpace);
            }
            return perfil;
        }
    }
}
