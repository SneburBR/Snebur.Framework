using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Snebur.Utilidade;

namespace Snebur.Imagem
{
    public abstract class BaseProcessarImagem : IDisposable
    {
        public const double DPI_PADRAO_VISUALIZACAO = 96;
        public const double DPI_PADRAO = 300;
        public const int QUALIDADE_PADRAO_SALVAR = 92;

        internal protected Stream Stream { get; private set; }

        private Size Tamanho { get; set; }

        public int ComprimentoMaximo { get; }
        public EnumLadoComprimento LadoComprimento { get; }

        public double Dpi { get; }
        public double Angulo { get; set; } = 0D;
        public bool ConverterPerfilOrigem { get; }

        public int LarguraImagemOriginal { get; private set; }
        public int AlturaImagemOriginal { get; private set; }

        public Size TamanhoImagemOriginal
        {
            get
            {
                return new Size(this.LarguraImagemOriginal, this.AlturaImagemOriginal);
            }

        }

        public int Largura { get; private set; }
        public int Altura { get; private set; }

        public Size TamanhoRedimensionado { get; private set; }
        public bool NaoAumentar { get; }
        public EnumOpcaoRedimensionar OpcaoRedimensionar { get; }
        public BitmapDecoder Decoder { get; private set; }
        public BitmapMetadata Metadata { get; protected set; }
        public ColorContext PerfilOrigem { get; private set; }
        public ColorContext PerfilDestino { get; private set; }
        public bool CarregarMetaData { get; set; }
        private bool FecharStream { get; } = false;

        public BaseProcessarImagem(string caminhoArquivo, int comprimentoMaximo, EnumLadoComprimento lado, double Dpi, bool converterPerfilOrigem)
        {
            this.Stream = StreamUtil.OpenRead(caminhoArquivo);
            this.FecharStream = true;
            this.CarregarMetaData = !SistemaUtil.IsWindowsXp;
        }

        public BaseProcessarImagem(Stream stream, int comprimentoMaximo, EnumLadoComprimento lado, double Dpi, bool converterPerfilOrigem)
        {
            this.Stream = stream;
            this.ComprimentoMaximo = comprimentoMaximo;
            this.LadoComprimento = lado;
            this.Dpi = Dpi;
            this.ConverterPerfilOrigem = converterPerfilOrigem;
            this.CarregarMetaData = !SistemaUtil.IsWindowsXp;
        }

        public BaseProcessarImagem(Stream stream, Size tamanho, EnumOpcaoRedimensionar opcaoRedimensionar, bool naoAumentar, double Dpi, bool converterPerfilOrigem)
        {
            this.Stream = stream;
            this.Tamanho = tamanho;
            this.Dpi = Dpi;
            this.ConverterPerfilOrigem = converterPerfilOrigem;
            this.Tamanho = tamanho;
            this.OpcaoRedimensionar = opcaoRedimensionar;
            this.CarregarMetaData = !SistemaUtil.IsWindowsXp;
            this.NaoAumentar = naoAumentar;
        }

        protected internal BitmapSource RetornarImagemInterno(Stream stream, BitmapCacheOption cacheOption, bool carregaImagemResutladoMemoria)
        {

            if (this.Stream.CanSeek)
            {
                this.Stream.Seek(0, SeekOrigin.Begin);
            }
            try
            {

                this.Decoder = DecoderUtil.RetornarDecoder(stream, cacheOption);
                //var cacheOption = decoder.RetornarCacheOption();

                var frame = this.Decoder.Frames.First();

                if (this.CarregarMetaData)
                {
                    this.Metadata = MetadataUtil.RetornarMetadata(frame, false);
                }

                this.PerfilOrigem = frame.RetornarPerfilOrigem();

                this.LarguraImagemOriginal = frame.PixelWidth;
                this.AlturaImagemOriginal = frame.PixelHeight;

                var source = frame as BitmapSource;


                if (ConfiguracaoTeste.SalvarImagemTemporaria)
                {
                    ImagemUtil.SalvarImagem(frame, $@"c:\temp\frame sRGB -{frame.PixelWidth} - {frame.PixelHeight} - {Guid.NewGuid()}.jpg");
                }


                this.PerfilDestino = PerfilIccUtil.RetornarPerfilNativo(EnumPerfilIcc.sRGB);
                var bitmapSourceMiniatura = frame as BitmapSource;

                if (frame.Format == PixelFormats.Indexed8)
                {
                    var formatoBgr24 = new FormatConvertedBitmap();
                    formatoBgr24.BeginInit();
                    formatoBgr24.Source = source;
                    formatoBgr24.DestinationFormat = PixelFormats.Bgr24;
                    formatoBgr24.EndInit();
                    source = formatoBgr24;
                }

                //BitmapSource sourcesRGB;
                //if (!this.PerfilOrigem.IsPerfilsRGB() && !SistemaUtil.IsWindowsXp)
                //{

                var sourcesRGB = new ColorConvertedBitmap();
                sourcesRGB.BeginInit();
                sourcesRGB.Source = source;
                sourcesRGB.SourceColorContext = this.PerfilOrigem;
                sourcesRGB.DestinationFormat = PixelFormats.Pbgra32;
                sourcesRGB.DestinationColorContext = this.PerfilDestino;
                sourcesRGB.EndInit();

                if (ConfiguracaoTeste.SalvarImagemTemporaria)
                {
                    ImagemUtil.SalvarImagem(sourcesRGB, $@"c:\temp\Source Pbgra32 sRGB -{sourcesRGB.PixelWidth} - {sourcesRGB.PixelHeight} - {Guid.NewGuid()}.jpg");
                }

                //sourcesRGB = corsRGB;
                //}
                //else
                //{
                //    sourcesRGB = source;
                //}

                var scalarX = this.RetornarScalarX();
                var scalarY = this.RetornarScalarY();

                if (this.NaoAumentar)
                {
                    if (scalarX > 1 && scalarY > 1)
                    {
                        scalarX = 1;
                        scalarY = 1;
                    }
                }

                if (!(scalarX == 1 && scalarY == 1))
                {
                    this.Metadata = MetadataUtil.ClonarMetadata(this.Metadata);
                }

                this.Largura = (int)(this.LarguraImagemOriginal * scalarX);
                this.Altura = (int)(this.AlturaImagemOriginal * scalarY);


                //var transformedSource = new TransformedBitmap(sourcesRGB, new ScaleTransform(scalarX, scalarY));

                //transformedSource.BeginInit();
                //if (SistemaUtil.IsWindowsXp)
                //{
                //    transformedSource.Source = sourcesRGB;
                //}
                //else
                //{
                //    transformedSource.Source = frame;
                //}
                //}
                var transformedSource = new TransformedBitmap();
                var transformGroup = new TransformGroup();
                transformedSource.BeginInit();
                transformedSource.Source = sourcesRGB;

                if (this.Angulo > 0)
                {
                    transformGroup.Children.Add(new RotateTransform(this.Angulo));
                }

                transformGroup.Children.Add(new ScaleTransform(scalarX, scalarY));
                transformedSource.Transform = transformGroup;
                transformedSource.EndInit();


                if (ConfiguracaoTeste.SalvarImagemTemporaria)
                {
                    ImagemUtil.SalvarImagem(transformedSource, $@"c:\temp\transformedSource sRGB -{transformedSource.PixelWidth} - {transformedSource.PixelHeight} - {Guid.NewGuid()}.jpg");
                }


                BitmapSource imagemRedimensionada = transformedSource;
                var cache = (carregaImagemResutladoMemoria) ? BitmapCacheOption.OnLoad : BitmapCacheOption.None;
                this.TamanhoRedimensionado = new Size(imagemRedimensionada.PixelWidth, imagemRedimensionada.PixelHeight);
                if (!this.ConverterPerfilOrigem || this.PerfilOrigem.IsPerfilsRGB() || SistemaUtil.IsWindowsXp)
                {


                    var imagemFinal = new CachedBitmap(transformedSource, BitmapCreateOptions.PreservePixelFormat |
                                                                          BitmapCreateOptions.IgnoreColorProfile,
                                                                          cache);

                    if (ConfiguracaoTeste.SalvarImagemTemporaria)
                    {
                        ImagemUtil.SalvarImagem(imagemFinal, $@"c:\temp\Imagem_Cache-{this.Largura} - {this.Altura} - {Guid.NewGuid()}.jpg");
                    }
                    if (carregaImagemResutladoMemoria)
                    {
                        imagemFinal.Freeze();
                    }
                    return this.RetornarBitmapSourceFinal(imagemFinal);
                }

                var corOrigem = new ColorConvertedBitmap();
                corOrigem.BeginInit();
                corOrigem.Source = imagemRedimensionada;
                corOrigem.SourceColorContext = this.PerfilDestino;
                corOrigem.DestinationFormat = source.Format;
                corOrigem.DestinationColorContext = this.PerfilOrigem;
                corOrigem.EndInit();

                var imagemFinalCorOrigem = new CachedBitmap(corOrigem,
                                                            BitmapCreateOptions.PreservePixelFormat |
                                                            BitmapCreateOptions.IgnoreColorProfile, cacheOption);


                return this.RetornarBitmapSourceFinal(imagemFinalCorOrigem);

            }
            catch (OutOfMemoryException ex)
            {
                if (cacheOption == BitmapCacheOption.None)
                {
                    return this.RetornarImagemErro(ex);
                }
                else
                {
                    return this.RetornarImagemInterno(stream, BitmapCacheOption.None, carregaImagemResutladoMemoria);
                }
            }
            catch (Exception ex)
            {
                return this.RetornarImagemErro(ex);
            }
        }



        private BitmapSource RetornarBitmapSourceFinal(BitmapSource bimtapSource)
        {
            if (bimtapSource.DpiX == this.Dpi || bimtapSource.DpiY == this.Dpi)
            {
                bimtapSource.Freeze();
                return bimtapSource;
            }
            try
            {
                var imagemFinal = BitmapSourceUtil.AjustarDpi(bimtapSource, this.Dpi);
                imagemFinal.Freeze();
                return imagemFinal;
            }
            catch (Exception)
            {
                bimtapSource.Freeze();
                return bimtapSource;
            }

        }

        protected BitmapCacheOption RetornarCacheOptionAutomatico()
        {
            return BitmapCacheOption.OnLoad;
        }

        protected virtual BitmapSource RetornarImagemErro(Exception ex)
        {
            throw ex;
        }

        #region Redimensioanr Scalar

        private double RetornarScalarX()
        {
            if (this.ComprimentoMaximo > 0)
            {
                return this.RetornarScalarComprimento();
            }
            var tamanhoAjustado = this.RetornarTamanhoRedimensioanr();
            if (!tamanhoAjustado.IsEmpty && tamanhoAjustado.Width > 0 && tamanhoAjustado.Height > 0)
            {
                return tamanhoAjustado.Width / (double)this.LarguraImagemOriginal;
            }
            return 1;
            
        }

        private double RetornarScalarY()
        {
            if (this.ComprimentoMaximo > 0)
            {
                return this.RetornarScalarComprimento();
            }
            var tamanhoAjustado = this.RetornarTamanhoRedimensioanr();
            if (!tamanhoAjustado.IsEmpty && tamanhoAjustado.Width > 0 && tamanhoAjustado.Height > 0)
            {
                return tamanhoAjustado.Height / (double)this.AlturaImagemOriginal;
            }
            return 1;
            
        }

        private double RetornarScalarComprimento()
        {
            switch (this.LadoComprimento)
            {
                case EnumLadoComprimento.Largura:

                    return this.ComprimentoMaximo / (double)this.LarguraImagemOriginal;

                case EnumLadoComprimento.Altura:

                    return this.ComprimentoMaximo / (double)this.AlturaImagemOriginal;

                default:

                    throw new Exception($"O LadoComprimento '{EnumUtil.RetornarDescricao(this.LadoComprimento)}' não suportado ");
            }
        }

        private Size RetornarTamanhoRedimensioanr()
        {
            switch (this.OpcaoRedimensionar)
            {
                case EnumOpcaoRedimensionar.Nenhum:

                    return this.Tamanho;

                case EnumOpcaoRedimensionar.Uniforme:

                    return ImagemUtil.RetornarTamanhoUniforme(this.LarguraImagemOriginal, this.AlturaImagemOriginal, this.Tamanho.Width, this.Tamanho.Height);

                case EnumOpcaoRedimensionar.UniformeParaPrencher:

                    return ImagemUtil.RetornarTamanhoUniformeParaPreencher(this.LarguraImagemOriginal, this.AlturaImagemOriginal, this.Tamanho.Width, this.Tamanho.Height);

                case EnumOpcaoRedimensionar.AjustarUniforme:

                    var tamanhoAjustado = this.RetornarTamanhoAjustado();
                    return ImagemUtil.RetornarTamanhoUniforme(this.LarguraImagemOriginal, this.AlturaImagemOriginal, tamanhoAjustado.Width, tamanhoAjustado.Height);

                case EnumOpcaoRedimensionar.AjustarUniformeParaPrencher:

                    var tamanhoAjustado2 = this.RetornarTamanhoAjustado();
                    return ImagemUtil.RetornarTamanhoUniformeParaPreencher(this.LarguraImagemOriginal, this.AlturaImagemOriginal, tamanhoAjustado2.Width, tamanhoAjustado2.Height);

                case EnumOpcaoRedimensionar.NaoRedimensionar:

                    return this.TamanhoImagemOriginal;

                default:

                    throw new ErroNaoSuportado($"Opçaõ de redimensionar não suportado {EnumUtil.RetornarDescricao(this.OpcaoRedimensionar)}");
            }

        }

        private Size RetornarTamanhoAjustado()
        {
            if (this.Tamanho.Width >= this.Tamanho.Height && this.LarguraImagemOriginal >= this.AlturaImagemOriginal)
            {
                return this.Tamanho;
            }
            return new Size(this.Tamanho.Height, this.Tamanho.Width);

        }

        #endregion



        #region Salvar

        public void Salvar(string caminhoArquivoDestino)
        {
            var caminhoTemp = CaminhoUtil.RetornarCaminhoArquivoTemporario(Path.GetDirectoryName(caminhoArquivoDestino));
            try
            {
                using (var stream = StreamUtil.CreateWrite(caminhoArquivoDestino))
                {
                    this.Salvar(stream);
                }
                ArquivoUtil.MoverArquivo(caminhoTemp, caminhoArquivoDestino);
            }
            catch (Exception)
            {
                if (File.Exists(caminhoTemp))
                {
                    ArquivoUtil.DeletarArquivo(caminhoTemp);
                }
                throw ;
            }
        }

        public void Salvar(Stream streamDestino)
        {
            var imagem = this.RetornarImagemInterno(this.Stream, BitmapCacheOption.None, false);
            this.SalvarInterno(imagem, streamDestino);
        }

        private void SalvarInterno(BitmapSource imagem, Stream streamDestino, bool clonarMetadadata = false)
        {
            try
            {
                if (clonarMetadadata)
                {
                    this.Metadata = MetadataUtil.ClonarMetadata(this.Metadata);
                }

                var encoder = EncoderUtil.RetornarEncoder(this.Decoder, QUALIDADE_PADRAO_SALVAR);
                var perfilSalvar = this.RetonarPerfilSalvar();
                var frame = BitmapFrame.Create(imagem, null, this.Metadata, perfilSalvar.RetornarColorContexts());
                encoder.Frames.Add(frame);
                encoder.Save(streamDestino);
                encoder = null;
                frame = null;

            }
            catch (Exception)
            {
                if (clonarMetadadata)
                {
                    throw;
                }
                StreamUtil.SetarPonteiroInicio(streamDestino);
                SalvarInterno(imagem, streamDestino, true);
            }
        }

        private ColorContext RetonarPerfilSalvar()
        {
            if (!this.ConverterPerfilOrigem || SistemaUtil.IsWindowsXp)
            {
                return PerfilIccUtil.RetornarPerfilNativo(EnumPerfilIcc.sRGB);
            }
            return this.PerfilOrigem;
        }



        #endregion

        public void Dispose()
        {
            this.PerfilOrigem = null;
            this.Metadata = null;
            this.Decoder = null;
            if (this.FecharStream)
            {
                if (this.Stream != null)
                {
                    this.Stream.Dispose();
                    this.Stream = null;
                }

            }

        }
    }
}
