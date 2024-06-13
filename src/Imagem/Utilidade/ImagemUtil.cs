
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Snebur.Dominio;
using Snebur.Imagens;

#if NET48
using Microsoft.WindowsAPICodePack.Shell;
#endif

namespace Snebur.Utilidade
{
    public partial class ImagemUtil
    {


        public static EnumTamanhoImagem[] TamanhosApresentacao => new EnumTamanhoImagem[] { EnumTamanhoImagem.Miniatura, EnumTamanhoImagem.Pequena, EnumTamanhoImagem.Media, EnumTamanhoImagem.Grande };

        #region Miniatura

#if NET45

        public async static Task<BitmapSource> RetornarMiniaturaAwait(string caminhoArquivo, double alturaMaxima = ConstantesImagemApresentacao.ALTURA_IMAGEM_MINIATURA)
        {
            return await Task.Factory.StartNew<BitmapSource>(() =>
            {
                return ImagemUtil.RetornarMiniatura(caminhoArquivo, alturaMaxima);
            });
        }

#endif


        public static BitmapSource RetornarMiniatura(string caminhoArquivo, double alturaMaxima = ConstantesImagemApresentacao.ALTURA_IMAGEM_MINIATURA, bool erroRetornarNull = false)
        {
            var extensaoArquivo = Path.GetExtension(caminhoArquivo).ToLower();
            try
            {
                if (ImagemUtil.ExtensoesSuportadas.Contains(extensaoArquivo))
                {
                    using (var fs = StreamUtil.OpenRead(caminhoArquivo))
                    {
                        return ImagemUtil.RetornarMiniatura(fs, alturaMaxima);
                    }
                }
                //else if (ExtensoesPsd.Contains(extensaoArquivo))
                //{
                //    var miniaturaPsd = ImagemUtil.RetornarMiniaturaPsd(caminhoArquivo, alturaMaxima);
                //    if (miniaturaPsd != null)
                //    {
                //        return miniaturaPsd;
                //    }
                //}

            }
            catch (UnauthorizedAccessException)
            {
                throw ;
            }
            catch (Exception)
            {
            }

            if (erroRetornarNull)
            {
                return null;
            }
            return ImagemUtil.RetornarMiniaturaSemImagem(caminhoArquivo, alturaMaxima);
        }

        public static BitmapSource RetornarMiniatura(Stream stream, double alturaMaxima)
        {
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            return ImagemUtil.RetornarImagemVisualizacao(stream, alturaMaxima, true);

            //var bmitmap = new BitmapImage();
            //bmitmap.BeginInit();
            //bmitmap.CacheOption = BitmapCacheOption.OnLoad;
            //bmitmap.DecodePixelHeight = (int)alturaMaxima;
            //bmitmap.StreamSource = stream;
            //bmitmap.EndInit();
            //bmitmap.Freeze();
            //return bmitmap;

            //var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
            //var frame = decoder.Frames.First();

            //var miniatura =RetornarMiniatura(frame, alturaMaxima);
            //frame = null;
            //System.GC.Collect();
            //System.GC.WaitForPendingFinalizers();
            //return miniatura;
        }

        public static BitmapSource RetornarMiniatura(BitmapSource imagem, double alturaMaxima)
        {
            var tamanhoUniforme = RetornarTamanhoUnifiormeAlturaMaxima(imagem.PixelWidth, imagem.PixelHeight, alturaMaxima);
            var scalarWidth = tamanhoUniforme.Width / imagem.PixelWidth;
            var scalarHeight = tamanhoUniforme.Height / imagem.PixelHeight;
            var transformaBitmap = new TransformedBitmap(imagem, new ScaleTransform(scalarWidth, scalarHeight));
            var cache = new CachedBitmap(transformaBitmap, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            cache.Freeze();
            return cache;

            //BitmapSourceUtil.AjustarDpi(cache, 96);
            //return cache;
        }

        public static BitmapSource RetornarMiniaturaSemImagem(string caminhoArquivo, double alturaMaxima)
        {
            var bmitmap = new BitmapImage();
            bmitmap.BeginInit();
            bmitmap.CacheOption = BitmapCacheOption.OnLoad;
            bmitmap.DecodePixelHeight = (int)alturaMaxima;
            bmitmap.StreamSource = ImagemUtil.RetornarStreamSemImagem(caminhoArquivo);
            //bmitmap.UriSource = new Uri("pack://application:,,,/Snebur.Imagem;component/Recursos/Imagens/sem_imagem.jpg", UriKind.RelativeOrAbsolute);
            bmitmap.EndInit();
            bmitmap.Freeze();
            return bmitmap;
        }

        public static BitmapSource RetornarMiniaturaPsd(string caminhoArquivo, double alturaMaxima)
        {
#if NET6_0_OR_GREATER == false

            var shellFile = ShellFile.FromFilePath(caminhoArquivo);
            var entensao = Path.GetExtension(caminhoArquivo);

            if (ImagemUtil.ExtensoesPsd.Contains(entensao))
            {
                try
                {
                    shellFile.Thumbnail.FormatOption = ShellThumbnailFormatOption.Default;
                    var biSource = shellFile.Thumbnail.ExtraLargeBitmapSource;
                    //var bitmap = BitmapSourceUtil.AjustarDpi(biSource, ProcessarImagemVisualizacao.DPI_PADRAO_VISUALIZACAO);
                    //var cache = new CachedBitmap(bitmap, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    return RetornarMiniatura(biSource, alturaMaxima);
                }
                catch
                {
                    //não faz nada
                }
            }
#endif
            return null;
        }

#endregion

        #region Stream miniatura

        public static MemoryStream RetornarStreamMiniatura(string caminhoArquivo, double alturaMaxima, bool erroRetornarNull = false)
        {
            try
            {
                var extensaoArquivo = Path.GetExtension(caminhoArquivo).ToLower();
                if (ExtensoesSuportadas.Contains(extensaoArquivo))
                {

                    using (var ms = new MemoryStream(File.ReadAllBytes(caminhoArquivo)))
                    {
                        return ImagemUtil.RetornarStreamResizeImageVisualizacao(caminhoArquivo, alturaMaxima);
                    }
                }
                //else if (ExtensoesPsd.Contains(extensaoArquivo))
                //{
                //    var imagemSource = ImagemUtil.RetornarMiniaturaPsd(caminhoArquivo, alturaMaxima);
                //    if (imagemSource != null)
                //    {
                //        return ImagemUtil.RetornarMemoryStream(imagemSource, QUALIDADE_PADRAO_THUMBNAIL);
                //    }
                //}
            }
            catch (Exception)
            {
                //Não foi possivel gerar miniatura de extensao suportada
            }
            if (erroRetornarNull)
            {
                return null;
            }
            var streamSemImagem = ImagemUtil.RetornarStreamSemImagem(caminhoArquivo);
            return ImagemUtil.RetornarStreamResizeImageVisualizacao(streamSemImagem, alturaMaxima, false, true);
        }

        public static MemoryStream RetornarStreamSemImagem(string caminho)
        {
            var extensao = Path.GetExtension(caminho);
            var caminhoRecurso = String.Format("Snebur.Imagem.Recursos.Icones{0}{0}.png", extensao);
            var streamRecurso = typeof(ImagemUtil).Assembly.GetManifestResourceStream(caminhoRecurso);
            if (streamRecurso != null)
            {
                return StreamUtil.RetornarMemoryStream(streamRecurso);
            }
            return ImagemUtil.RetornarStreamSemImagem();
        }

        public static MemoryStream RetornarStreamSemImagem()
        {
            return StreamUtil.RetornarMemoryStream(typeof(ImagemUtil).Assembly.GetManifestResourceStream("Snebur.Imagem.Recursos.Imagens.sem_imagem.jpg"));
        }

        public static MemoryStream RetornarStreamSemImagemPng()
        {
            return StreamUtil.RetornarMemoryStream(typeof(ImagemUtil).Assembly.GetManifestResourceStream("Snebur.Imagem.Recursos.Imagens.sem_imagem.png"));
        }

        #endregion

        #region Retornar BitmapImage 

        public static BitmapImage RetornarBitmapImage(string caminhoArquivo)
        {
            var bitmapImage = new BitmapImage();
            var ms = new MemoryStream(File.ReadAllBytes(caminhoArquivo));
            return ImagemUtil.RetornarBitmapImage(ms);
        }

        public static BitmapImage RetornarBitmapImage(Stream stream)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();
            return bitmapImage;
        }

        public static BitmapImage RetornarBitmapImage(FileInfo arquivo)
        {
            var bi = new BitmapImage();
            using (FileStream f = arquivo.OpenRead())
            {
                bi.BeginInit();
                bi.StreamSource = f;
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.EndInit();
                bi.Freeze();
            }
            return bi;
        }

        public static BitmapSource RetornarBitmapImagem(string caminhoArquivo)
        {
            using (var fs = StreamUtil.OpenRead(caminhoArquivo))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = fs;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                return bitmap;
            }

        }

        public static BitmapSource RetornarBitmapImagem(byte[] bytesArquivo)
        {
            using (var ms = new MemoryStream(bytesArquivo))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                return bitmap;
            }

        }

        #endregion

        #region Redimensionar

        /// <summary>
        /// Usar sempre no Workeer thread
        /// </summary>
        /// <param name="caminho"></param>
        /// <param name="larguraMaxima"></param>
        /// <param name="alturaMaxima"></param>
        /// <returns></returns>
        /// 
        public static MemoryStream RetornarStreamResizeImageVisualizacao(string caminhoOrigem, double alturaMaxima, bool usarBitmapImage = true)
        {
            if (File.Exists(caminhoOrigem))
            {
                try
                {
                    using (var fs = StreamUtil.OpenRead(caminhoOrigem))
                    {
                        return RetornarStreamResizeImageVisualizacao(fs, alturaMaxima, usarBitmapImage);
                    }
                }
                catch (ErroImagemCorrompida)
                {
                    throw;
                }
                catch (ErroImagem)
                {
                    throw;
                }
                catch (Exception erro)
                {
                    throw new ErroImagem(String.Format("Não foi possível redimensionar a imagem : caminho : {0}", caminhoOrigem), erro);
                }
            }
            return null;
        }

        public static MemoryStream RetornarStreamResizeImageVisualizacao(Stream stream, 
                                                                         double alturaMaxima, 
                                                                         bool usarImagemBitmap = true, 
                                                                         bool permitirJpeg = false)
        {
            try
            {
                try
                {
                    Stream streamImagem = stream;
                    if (!PngResolver.UsarPng && permitirJpeg)
                    {
                        streamImagem = PngResolver.RetornarStreamComoJpeg(stream);
                    }

                    if (stream.CanSeek)
                    {
                        streamImagem.Seek(0, SeekOrigin.Begin);
                    }

                    var imagem = ImagemUtil.RetornarImagemVisualizacao(streamImagem, alturaMaxima, usarImagemBitmap);

                    BitmapEncoder encoder;
                    if (PngResolver.UsarPng && permitirJpeg)
                    {
                        encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(imagem));
                    }
                    else
                    {
                        encoder = new JpegBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(imagem));
                    }

                    var msRetorno = new MemoryStream();
                    encoder.Save(msRetorno);
                    msRetorno.Seek(0, SeekOrigin.Begin);
                    return msRetorno;

                }
                catch (Exception ex)
                {
                    LogUtil.ErroAsync(ex);
                    if (!PngResolver.UsarPng || !permitirJpeg)
                    {
                        throw;
                    }
                    PngResolver.UsarPng = false;
                    return ImagemUtil.RetornarStreamResizeImageVisualizacao(stream, alturaMaxima, usarImagemBitmap, permitirJpeg);
                }
            }
            catch (ErroImagemCorrompida)
            {
                throw;
            }
            catch (Exception erro)
            {
                throw erro;
            }
        }

#if NET45

        public static async Task<Exception> AbrirImagemInteirnaMemoria(string caminhoArquivo)
        {
            Exception retorno = await Task.Factory.StartNew<Exception>(() =>
            {
                Exception erro = null;
                try
                {
                    using (var fs = StreamUtil.OpenRead(caminhoArquivo))
                    {
                        BitmapDecoder.Create(fs, BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.OnLoad);
                    }
                }
                catch (Exception ex)
                {
                    erro = ex;
                }
                finally
                {
                    GC.Collect();
                    GC.CancelFullGCNotification();
                }
                return erro;
            });
            GC.Collect();
            GC.CancelFullGCNotification();
            Thread.Sleep(100);
            return retorno;
        }
        public static async Task<BitmapSource> RetornarImagemVisualizacaoAwait(string caminhoArquivo, double alturaMaxima, bool usarBitmapImage = true, double angulo = 0)
        {
            return await Task.Factory.StartNew<BitmapSource>(() =>
            {
                BitmapSource retorno = ImagemUtil.RetornarImagemVisualizacao(caminhoArquivo, alturaMaxima, usarBitmapImage, angulo);
                return retorno;
            });
        }

#endif

        public static BitmapSource RetornarImagemVisualizacao(string caminhoArquivo, double alturaMaxima, bool usarBitmapImage = true, double angulo = 0)
        {
            using (var fsOrigem = StreamUtil.OpenRead(caminhoArquivo))
            {
                return ImagemUtil.RetornarImagemVisualizacao(fsOrigem, alturaMaxima, usarBitmapImage);
            }
        }

        public static BitmapSource RetornarImagemVisualizacao(Stream stream, double alturaMaxima, bool usarBitmapImage = true, double angulo = 0)
        {
            using (var imagemVisualziacao = new ProcessarImagemVisualizacao(stream, (int)alturaMaxima, usarBitmapImage))
            {
                imagemVisualziacao.Angulo = angulo;
                return imagemVisualziacao.RetornarImagemVisualizacao();
            }
        }

        public static BitmapSource RetornarImagemVisualizacao(byte[] bytesImagem, double alturaMaxima, bool usarBitmapImage = true, double angulo = 0)
        {
            using (var msOrigem = new MemoryStream(bytesImagem))
            {
                return ImagemUtil.RetornarImagemVisualizacao(msOrigem, alturaMaxima, usarBitmapImage);
            }
        }

        public static BitmapSource RetornarImagemVisualizacao(string caminhoArquivo, double comprimentoMaximo, EnumLadoComprimento ladoComprimento, bool usarBitmapImage = true, double angulo = 0)
        {
            using (var fsOrigem = StreamUtil.OpenRead(caminhoArquivo))
            {
                return ImagemUtil.RetornarImagemVisualizacao(fsOrigem, comprimentoMaximo, usarBitmapImage);
            }
        }

        public static BitmapSource RetornarImagemVisualizacao(Stream stream, double comprimentoMaximo, EnumLadoComprimento ladoComprimento, bool usarBitmapImage = true, double angulo = 0)
        {
            using (var imagemVisualziacao = new ProcessarImagemVisualizacao(stream, (int)comprimentoMaximo, ladoComprimento, usarBitmapImage))
            {
                imagemVisualziacao.Angulo = angulo;
                return imagemVisualziacao.RetornarImagemVisualizacao();
            }
        }

        public static BitmapSource RetornarImagemVisualizacao(byte[] bytesImagem, double comprimentoMaximo, EnumLadoComprimento ladoComprimento, bool usarBitmapImage = true, double angulo = 0)
        {
            using (var msOrigem = new MemoryStream(bytesImagem))
            {
                return ImagemUtil.RetornarImagemVisualizacao(msOrigem, comprimentoMaximo, usarBitmapImage);
            }
        }

        public static BitmapSource RetornarImagemVisualizacao(string caminhoArquivo, Size tamanho, EnumOpcaoRedimensionar opcaoRedimensionar, double angulo = 0)
        {
            using (var fsOrigem = StreamUtil.OpenRead(caminhoArquivo))
            {
                return ImagemUtil.RetornarImagemVisualizacao(fsOrigem, tamanho, opcaoRedimensionar, angulo);
            }

        }

        public static BitmapSource RetornarImagemVisualizacao(Stream stream, Size tamanho, EnumOpcaoRedimensionar opcaoRedimensionar, double angulo = 0)
        {
            using (var imagemVisualziacao = new ProcessarImagemVisualizacao(stream, tamanho, opcaoRedimensionar, true))
            {
                imagemVisualziacao.Angulo = angulo;
                return imagemVisualziacao.RetornarImagemVisualizacao();
            }
        }


        public static BitmapSource RedimensionarImagemUniforme(BitmapSource imagem, double alturaMaxima, bool aumentar = true, bool carregar = true)
        {

            var scalarY = alturaMaxima / imagem.PixelHeight;
            if (aumentar || (scalarY < 1))
            {
                imagem = new TransformedBitmap(imagem, new ScaleTransform(scalarY, scalarY));
            }
            if (carregar)
            {
                imagem = new CachedBitmap(imagem, BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.OnLoad);
                imagem.Freeze();
            }
            return imagem;

        }

        public static BitmapSource RedimensionarImagemUniforme(BitmapSource imagem, double largura, double altura, bool aumentar = false)
        {
            var tamanhoRedimencionar = ImagemUtil.RetornarTamanhoUniforme(imagem.PixelWidth, imagem.PixelHeight, largura, altura);
            var scalarX = tamanhoRedimencionar.Width / imagem.PixelWidth;
            var scalarY = tamanhoRedimencionar.Height / imagem.PixelHeight;

            if (aumentar || (scalarX < 1 && scalarY < 1))
            {
                var tbmp = new TransformedBitmap(imagem, new ScaleTransform(scalarX, scalarY));
                return tbmp;
            }
            return imagem;
        }

        public static BitmapSource RedimensionarImagemScalar(BitmapSource imagem, double ratioX, double ratioY)
        {
            var tbmp = new TransformedBitmap(imagem, new ScaleTransform(ratioX, ratioY));
            return new CachedBitmap(tbmp, BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.OnLoad);
            //var imagemRedimencionada = BitmapFrame.Create(tbmp);
            //return imagemRedimencionada;
        }

        private static WriteableBitmap RetornarWriteableBitmap(BitmapSource imagem)
        {
            if (imagem is WriteableBitmap writeable)
            {
                return writeable;
            }
            return new WriteableBitmap(imagem);
        }




        #endregion

        #region Calcular tamanho uniforme

        public static Size RetornarTamanhoUniforme(Size tamanhoImagem, Size tamanhoDestino)
        {
            return RetornarTamanhoUniforme(tamanhoImagem.Width, tamanhoImagem.Height, tamanhoDestino.Width, tamanhoDestino.Height);
        }

        public static Size RetornarTamanhoUniforme(double larguraImagem, double alturaImagem, double largura, double altura)
        {
            double novaLargura = 0;
            double novaAltura = 0;

            if (larguraImagem > alturaImagem)
            {
                //IMAGEM NA HORIZONTAL
                novaLargura = largura;
                novaAltura = alturaImagem * novaLargura / larguraImagem;

                if (novaAltura > altura)
                {
                    novaAltura = altura;
                    novaLargura = larguraImagem * novaAltura / alturaImagem;
                }
            }
            else if (alturaImagem > larguraImagem)
            {
                //IMAGEM NA VERTICAL
                novaAltura = altura;
                novaLargura = larguraImagem * novaAltura / alturaImagem;

                if (novaLargura > largura)
                {
                    novaLargura = largura;
                    novaAltura = alturaImagem * novaLargura / larguraImagem;
                }
            }
            else if (larguraImagem == alturaImagem)
            {
                //IMAGEM QUADRADA ' SELECIONAR O MENOR LADO
                if (altura > largura)
                {
                    novaLargura = largura;
                    novaAltura = largura;
                }
                else
                {
                    novaLargura = altura;
                    novaAltura = altura;
                }
            }
            return new System.Windows.Size(novaLargura, novaAltura);
        }

        public static Size RetornarTamanhoUniformeParaPreencher(Size tamanhoImagem, Size tamanhoDestino)
        {
            return RetornarTamanhoUniformeParaPreencher(tamanhoImagem.Width, tamanhoImagem.Height, tamanhoDestino.Width, tamanhoDestino.Height);
        }

        public static Size RetornarTamanhoUniformeDentro(Size tamanhoImagem, Size tamanhoDestino)
        {
            return RetornarTamanhoUniformeParaPreencher(tamanhoImagem.Width, tamanhoImagem.Height, tamanhoDestino.Width, tamanhoDestino.Height);
        }

        public static Size RetornarTamanhoUniformeFora(Size tamanhoImagem, Size tamanhoDestino)
        {
            return RetornarTamanhoUniformeParaPreencher(tamanhoImagem.Width, tamanhoImagem.Height, tamanhoDestino.Width, tamanhoDestino.Height);
        }


        public static Size RetornarTamanhoUniformeParaPreencher(double larguraImagem, double alturaImagem, double largura, double altura)
        {
            double novaLargura = 0;
            double novaAltura = 0;

            if (larguraImagem > alturaImagem)
            {
                //IMAGEM NA HORIZONTAL

                novaLargura = largura;
                novaAltura = alturaImagem * novaLargura / larguraImagem;

                if (novaAltura < altura)
                {
                    novaAltura = altura;
                    novaLargura = larguraImagem * novaAltura / alturaImagem;
                }
            }
            else if (alturaImagem > larguraImagem)
            {
                //IMAGEM NA VERTICAL

                novaAltura = altura;
                novaLargura = larguraImagem * novaAltura / alturaImagem;

                if (novaLargura < largura)
                {
                    novaLargura = largura;
                    novaAltura = alturaImagem * novaLargura / larguraImagem;
                }
            }
            else if (larguraImagem == alturaImagem)
            {
                //IMAGEM QUADRADA ' SELECIONAR O MENOR LADO
                if (altura < largura)
                {
                    novaLargura = largura;
                    novaAltura = largura;
                }
                else
                {
                    novaLargura = altura;
                    novaAltura = altura;
                }
            }
            return new System.Windows.Size(novaLargura, novaAltura);
        }

        public static Size RetornarTamanhoUnifiormeAlturaMaxima(double larguraImagem, double alturaImagem, double alturaMaxima)
        {
            double novaLargura, novaAltura;
            novaAltura = alturaMaxima;
            novaLargura = larguraImagem * novaAltura / alturaImagem;
            return new Size(novaLargura, novaAltura);
        }

        public static Size RetornarTamanhoUnifiormeLarguraMaxima(double larguraImagem, double alturaImagem, double larguraMaxima)
        {
            double novaLargura, novaAltura;
            novaLargura = larguraMaxima;
            novaAltura = alturaImagem * novaLargura / larguraImagem;
            return new Size(novaLargura, novaAltura);
        }

        #endregion

        /// <summary>
        /// Salvar a imagem no formato JPEG
        /// </summary>
        /// 
        public static void SalvarImagem(BitmapSource imagem, string caminho, int qualidade = 100)
        {
            ImagemUtil.SalvarImagem(imagem, caminho, EnumFormatoImagem.JPEG, qualidade);
        }

        public static void SalvarImagem(BitmapSource imagem, string caminho, EnumFormatoImagem formato, int qualidade = 100)
        {
            using (var ms = RetornarMemoryStream(imagem, formato, qualidade))
            {
                File.WriteAllBytes(caminho, ms.ToArray());
            }
        }


        /// <summary>
        /// Salvar a imagem no formato JPEG
        /// </summary>
        public static MemoryStream RetornarMemoryStream(BitmapSource imagem, EnumFormatoImagem formato, int qualidade = 100)
        {
            var ms = new MemoryStream();
            if (formato == EnumFormatoImagem.PNG && PngResolver.UsarPng)
            {
                try
                {
                    var pngEncoder = new PngBitmapEncoder();
                    pngEncoder.Frames.Add(BitmapFrame.Create(imagem));
                    pngEncoder.Save(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    return ms;
                }
                catch (Exception)
                {
                    PngResolver.UsarPng = false;
                }
            }

            var encoder = new JpegBitmapEncoder()
            {
                QualityLevel = qualidade
            };

            encoder.Frames.Add(BitmapFrame.Create(imagem));
            encoder.Save(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;

        }

        //public static void SalvarBitmap(string caminhoArquivo, System.Drawing.Bitmap bitmap)
        //{
        //    ImagemUtil.SalvarBitmap(caminhoArquivo, bitmap, QUALIDADE_BITMAP);
        //}

        //public static void SalvarBitmap(string caminhoArquivo, System.Drawing.Bitmap bitmap, int qualidade)
        //{
        //    using (var ms = new MemoryStream())
        //    {
        //        using (var fs = File.OpenRead(caminhoArquivo))
        //        {
        //            var original = new JpegBitmapDecoder(fs, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
        //            var bitmapImage = BitmapUtil.BitmapToBitmapImage(bitmap);
        //            var encoder = new JpegBitmapEncoder();
        //            encoder.QualityLevel = qualidade;
        //            encoder.Frames.Add(BitmapFrame.Create(bitmapImage, null, original.Frames[0].Metadata as BitmapMetadata, original.Frames[0].ColorContexts));
        //            encoder.Save(ms);
        //            ms.Flush();
        //            ms.Seek(0, System.IO.SeekOrigin.Begin);
        //        }
        //        StreamUtil.SalvarStream(caminhoArquivo, ms);

        //    }
        //}

        public static void LimparBitmapImage(BitmapImage bitmap)
        {
            //var streamAtual = bitmap.StreamSource;
            //bitmap.BeginInit();
            //bitmap.StreamSource = BitmapUtil.RetornarStreamImagemVazia();
            //bitmap.EndInit();
            //if(streamAtual!= null)
            //{
            //    streamAtual.Dispose();
            //}
        }

        public static Size RetornarDimensao(string caminhoArquivo)
        {
            return DimensaoImagemUtil.RetornarDimensaoImagem(caminhoArquivo);
        }


        #region Extensoes

        public static EnumFormatoImagem RetornarFormatadoArquivo(string caminhoArquivo)
        {
            return ImagemUtil.RetornarFormatoExtensao(new FileInfo(caminhoArquivo));
        }

        public static BitmapSource RotacionarImagem(BitmapSource imagem, int angulo, bool caregar = true)
        {
            imagem = new TransformedBitmap(imagem, new RotateTransform(angulo));
            if (caregar)
            {
                imagem = new CachedBitmap(imagem, BitmapCreateOptions.IgnoreColorProfile | BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                imagem.Freeze();
            }
            return imagem;
        }

        public static void AjustarDpi(string caminhoArquivo, double dpi, bool forcar, bool ignorarErro)
        {
            AjustarDpiInterno(caminhoArquivo, dpi, forcar, ignorarErro, false);

        }
        private static void AjustarDpiInterno(string caminhoArquivo, double dpi, bool forcar, bool ignorarErro, bool clonarMetadata)
        {
            var caminhoBackup = CaminhoUtil.RetornarCaminhoArquivoBackup(caminhoArquivo);

            try
            {
                ArquivoUtil.CopiarArquivo(caminhoArquivo, caminhoBackup);

                //System.Threading.Thread.Sleep(5000);
                //System.Threading.Thread.Sleep(5000);

                using (var streamOrigem = new MemoryStream(File.ReadAllBytes(caminhoArquivo)))
                {

                    var decoder = DecoderUtil.RetornarDecoder(streamOrigem);
                    var frame = decoder.Frames.First();

                    var encoder = new JpegBitmapEncoder();
                    encoder.QualityLevel = 100;

                    var perfilOrigem = frame.RetornarPerfilOrigem();
                    var metadata = MetadataUtil.RetornarMetadata(frame, clonarMetadata);

                    BitmapSource source = frame;

                    if (frame.DpiX < dpi || frame.DpiY < dpi || forcar)
                    {
                        source = BitmapSourceUtil.AjustarDpi(frame, dpi);
                    }

                    encoder.Frames.Add(BitmapFrame.Create(source, null, metadata, perfilOrigem.RetornarColorContexts()));
                    using (var fsDestino = StreamUtil.CreateWrite(caminhoArquivo))
                    {
                        encoder.Save(fsDestino);
                    }

                    decoder = null;
                    frame = null;

                    System.GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
            catch (Exception)
            {
                if (clonarMetadata)
                {
                    if (File.Exists(caminhoBackup))
                    {
                        ArquivoUtil.CopiarArquivo(caminhoBackup, caminhoArquivo);
                    }

                    if (!ignorarErro)
                    {
                        throw;
                    }
                }
                else
                {
                    AjustarDpiInterno(caminhoArquivo, dpi, forcar, ignorarErro, true);
                }


            }
            finally
            {
                if (File.Exists(caminhoArquivo) && File.Exists(caminhoBackup))
                {
                    ArquivoUtil.DeletarArquivo(caminhoBackup);
                }
                DiretorioUtil.ExcluirDiretorio(Path.GetDirectoryName(caminhoBackup), false, true);
            }
        }

        #endregion

        public static bool IsArquivoBinarioJpeg(string caminhoArquivo)
        {
            if (!File.Exists(caminhoArquivo))
            {
                return false;
            }
            try
            {
                using (var fs = StreamUtil.OpenRead(caminhoArquivo))
                {
                    return IsStreamJpeg(fs);
                }
            }
            catch
            {
                return IsArquivoBinarioImagem(caminhoArquivo);
            }
        }

        public static bool IsStreamJpeg(Stream stream)
        {
            using (var br = new BinaryReader(stream))
            {
                var soi = br.ReadUInt16();  // Start of Image (SOI) marker (FFD8)
                var marker = br.ReadUInt16(); // JFIF marker (FFE0) EXIF marker (FFE1)
                var markerSize = br.ReadUInt16(); // size of marker data (incl. marker)
                var four = br.ReadUInt32(); // JFIF 0x4649464a or Exif  0x66697845

                var isJpeg = soi == 0xd8ff && (marker & 0xe0ff) == 0xe0ff;
                var isExif = isJpeg && four == 0x66697845;
                var isJfif = isJpeg && four == 0x4649464a;

                //if (isJpeg)
                //{
                //    if (isExif)
                //    {
                //        Console.WriteLine("EXIF: {0}", caminhoArquivo);
                //    }
                //    else if (isJfif)
                //    {
                //        Console.WriteLine("JFIF: {0}", caminhoArquivo);
                //    }
                //    else
                //    {
                //        Console.WriteLine("JPEG: {0}", caminhoArquivo);
                //    }
                //}

                return isJpeg || isJfif;
            }
        }

        public static bool IsArquivoBinarioImagem(string caminhoArquivo)
        {
            if (File.Exists(caminhoArquivo))
            {
                try
                {
                    using (var fs = StreamUtil.OpenRead(caminhoArquivo))
                    {
                        return IsStreamImagem(fs);
                    }
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        public static bool IsStreamImagem(Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            var decoder = DecoderUtil.RetornarDecoder(stream, BitmapCacheOption.None);
            var frame = decoder.Frames[0];
            return frame.PixelHeight > 0 && frame.PixelWidth > 0;
        }

        //public static bool IsArquivoBinarioImagem(string caminhoArquivo)
        //{
        //    if (File.Exists(caminhoArquivo))
        //    {
        //        using (var fs = StreamUtil.OpenRead(caminhoArquivo))
        //        {
        //            try
        //            {
        //                var decoder = DecoderUtil.RetornarDecoder(fs, BitmapCacheOption.None);
        //                var frame = decoder.Frames[0];
        //                return frame.PixelHeight > 0 && frame.PixelWidth > 0;
        //            }
        //            catch
        //            {
        //                return false;
        //            }
        //        }
        //    }
        //    return false;
        //}


        public static EnumTamanhoImagem RetornarTamanhoImagem(IImagem imagem, Dimensao dimensaoRecipiente)
        {

            if (dimensaoRecipiente.IsEmpty || dimensaoRecipiente.Largura <= 0)
            {
                throw new Erro("A dimensão não foi definida ou é invalida");
            }
            if (dimensaoRecipiente.Altura <= 0)
            {
                dimensaoRecipiente.Altura = dimensaoRecipiente.Largura;
            }

            var tolerancia = 1.2;
            var largura = dimensaoRecipiente.Largura * tolerancia;
            var altura = dimensaoRecipiente.Altura * tolerancia;

            if (largura < imagem.DimensaoImagemMiniatura.Largura || altura < imagem.DimensaoImagemMiniatura.Altura)
            {
                return EnumTamanhoImagem.Miniatura;
            }

            if (largura < imagem.DimensaoImagemPequena.Largura || altura < imagem.DimensaoImagemPequena.Altura)
            {
                return EnumTamanhoImagem.Pequena;
            }

            if (largura < imagem.DimensaoImagemMedia.Largura || altura < imagem.DimensaoImagemMedia.Altura)
            {
                return EnumTamanhoImagem.Media;
            }

            if (largura < imagem.DimensaoImagemGrande.Largura || altura < imagem.DimensaoImagemGrande.Altura)
            {
                return EnumTamanhoImagem.Grande;
            }

            return EnumTamanhoImagem.Impressao;
        }

    }

}
