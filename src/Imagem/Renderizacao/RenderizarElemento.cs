using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Snebur.Utilidade;

namespace Snebur.Imagens.Renderizacao
{
    public class RenderizarElemento : IDisposable
    {
        private Action<RenderizarElemento, Exception> CallbackConcluido;

        private InformacaoElementoOrigem InformacaoElementoOrigem;

        public int TotalPartes { get; set; }

        public int ParteAtual { get; set; }

        public double Progresso { get; }

        public Stream StreamDestino;

        public event ProgressoEventHandler ProgressoAlteado;

        public Size TamanhoRenderizacao { get; set; }

        private string CaminhoArquivoTemporario { get; set; }
        private Stream StreamTemporaria;
        private BinaryWriter BinaryWriter;

        public RenderizarElemento()
        {

        }

        public void RenderizarAsync(InformacaoElementoOrigem informacaoElementoOrigem, string caminhoDestino, Action<RenderizarElemento, Exception> callbackConcluido)
        {
            var streamDestino = new FileStream(caminhoDestino, FileMode.Create, FileAccess.Write, FileShare.Write);
            this.RenderizarAsync(informacaoElementoOrigem, streamDestino, callbackConcluido);
        }

        public void RenderizarAsync(InformacaoElementoOrigem informacaoElementoOrigem, Stream streamDestino, Action<RenderizarElemento, Exception> callbackConcluido)
        {
            this.InformacaoElementoOrigem = informacaoElementoOrigem;
            this.CallbackConcluido = callbackConcluido;
            this.StreamDestino = streamDestino;

            this.CaminhoArquivoTemporario = Path.GetTempFileName();
            this.StreamTemporaria = new FileStream(this.CaminhoArquivoTemporario, FileMode.Create, FileAccess.Write);
            this.BinaryWriter = new BinaryWriter(this.StreamTemporaria);

            ThreadUtil.ExecutarStaAsync(this.IniciarRenderizacao);
        }

        private void IniciarRenderizacao()
        {
            this.TotalPartes = this.RetornarTotalPartes();
            this.ParteAtual = 1;
            this.TamanhoRenderizacao = this.InformacaoElementoOrigem.TamanhoRenderizacao;
            this.RenderizarProximaParteAsync();
        }

        private void RenderizarProximaParteAsync()
        {
            ThreadUtil.ExecutarStaAsync(this.RenderizarProximaParte);

            GC.Collect();
            GC.WaitForFullGCApproach();
        }

        private void RenderizarProximaParte()
        {
            var gridConteudo = new Grid()
            {
                Width = this.TamanhoRenderizacao.Width,
                MinWidth = this.TamanhoRenderizacao.Width,
                MaxWidth = this.TamanhoRenderizacao.Width,

                Height = this.TamanhoRenderizacao.Height,
                MinHeight = this.TamanhoRenderizacao.Height,
                MaxHeight = this.TamanhoRenderizacao.Height,
                Background = Brushes.White
            };

            var elemento = this.InformacaoElementoOrigem.FuncaoRetornarElementoRenderizacao.Invoke();
            gridConteudo.Children.Add(elemento);

            var alturaParte = (int)Math.Round(this.TamanhoRenderizacao.Height / this.TotalPartes);
            if (this.ParteAtual == this.TotalPartes)
            {
                alturaParte = (int)this.TamanhoRenderizacao.Height - (this.TotalPartes - 1) * alturaParte;
            }

            var tamanhoParte = new Size(this.TamanhoRenderizacao.Width, alturaParte);
            var posicaoY = ((this.ParteAtual - 1) * alturaParte * -1);

            elemento.Margin = new Thickness(0, posicaoY, 0, 0);
            elemento.Measure(this.TamanhoRenderizacao);
            elemento.Arrange(new Rect(new Point(0, 0), this.TamanhoRenderizacao));
            elemento.UpdateLayout();

            gridConteudo.Measure(tamanhoParte);
            gridConteudo.Arrange(new Rect(new Point(0, 0), tamanhoParte));
            gridConteudo.UpdateLayout();

            var imagemRenderizada = new RenderTargetBitmap((int)tamanhoParte.Width, (int)tamanhoParte.Height, SizeEx.DPI_VISUALIZACAO, SizeEx.DPI_VISUALIZACAO, PixelFormats.Pbgra32);
            RenderOptions.SetBitmapScalingMode(imagemRenderizada, BitmapScalingMode.Linear);
            imagemRenderizada.Render(gridConteudo);

            var pixelsParte = imagemRenderizada.GetPixels(EnumPixelFormato.Rgba);
            var bmdEecoder = new BmpEncoder();
            //bmdEecoder.Salvar(imagemRenderizada, @"C:\Temp\Renderizar\bmp.bmp");
            bmdEecoder.SalvarPixels(this.BinaryWriter, (int)this.TamanhoRenderizacao.Width, (int)this.TamanhoRenderizacao.Height, pixelsParte, (int)tamanhoParte.Height, this.ParteAtual, this.TotalPartes);

            this.BinaryWriter.Flush();
            if (this.BinaryWriter.BaseStream is FileStream fs)
            {
                fs.Flush(true);
            }
            imagemRenderizada.Clear();
            imagemRenderizada = null;

            elemento.Clear();
            gridConteudo.Clear();
            elemento = null;
            gridConteudo = null;

            //gridConteudo.Measure(new Size(1, 1));
            //gridConteudo.Arrange(new Rect(new Point(0, 0), new Size(1, 1)));
            //gridConteudo.UpdateLayout();
            //gridConteudo = null;
            ThreadUtil.ExecutarAsync(this.LiberarMemoria);
        }

        private void NotificarProgresso()
        {
            var progresso = (this.ParteAtual / (double)this.TotalPartes) * 100;
            var args = new ProgressoEventArgs(progresso);

            ThreadUtil.ExecutarAsync(() =>
            {
                this.ProgressoAlteado(this, new ProgressoEventArgs(progresso));
            });
        }

        private void LiberarMemoria()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            System.Threading.Thread.Sleep(500);

            this.ParteAtual += 1;
            if (this.ParteAtual <= this.TotalPartes)
            {

                this.RenderizarProximaParteAsync();
            }
            else
            {
                this.SalvarImagemFinalAsync();
            }
        }

        private void SalvarImagemFinalAsync()
        {
            ThreadUtil.ExecutarAsync(this.SalvarImagemFinal);
        }
        private void SalvarImagemFinal()
        {
            this.BinaryWriter.Flush();
            this.BinaryWriter.Dispose();
            this.StreamTemporaria.Dispose();
            this.BinaryWriter = null;
            this.StreamTemporaria = null;

            using (var fs = new FileStream(this.CaminhoArquivoTemporario, FileMode.Open, FileAccess.Read))
            {
                var imagmeFinal = BitmapDecoder.Create(fs, BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.None);

                var frame = BitmapFrame.Create(imagmeFinal.Frames.First());
                using (var fsDestino = new BufferedStream(this.StreamDestino, 16 * 1024))
                {
                    var jpgEncoder = new JpegBitmapEncoder();
                    jpgEncoder.QualityLevel = 100;
                    jpgEncoder.Frames.Add(frame);
                    jpgEncoder.Save(fsDestino);
                }
            }
        }

        private int RetornarTotalPartes()
        {
            // calcular total de partes
            return 10;
        }

        public void Dispose()
        {
            this.StreamTemporaria?.Dispose();
            this.BinaryWriter?.Dispose();
            ArquivoUtil.DeletarArquivo(this.CaminhoArquivoTemporario);
            //throw new NotImplementedException();
        }
    }

    public class InformacaoElementoOrigem
    {
        public Func<FrameworkElement> FuncaoRetornarElementoRenderizacao { get; set; }
        public Size TamanhoRenderizacao { get; set; }
        public List<InformacaoImagem> Imagens { get; } = new List<InformacaoImagem>();
    }

    public class InformacaoImagem
    {
        //public Image ElementoImagem { get; }
        public string CaminhoImagem { get; }
        public double DistanciaTopo { get; }
        public Size TamanhoImagem { get; }
    }
    public class InfoParte
    {
        public int TotalPartes { get; }
        public int ParteAtual { get; }
        public int AlturaParte { get; }
        public int AlturaImagem { get; }
    }

    public static class SizeEx
    {
        public static int DPI_RENDERIZAR = 300;
        public static int DPI_VISUALIZACAO = 96;
        //public static int DPI_PADRAO_SALVAR = 300;

        public static Size ToSizePixels(this Size tamanho)
        {
            return new Size(tamanho.LarguraPixel(), tamanho.AlturuaPixel());
        }
        public static Size ToSizePixelsVisualizacao(this Size tamanho)
        {
            return new Size(tamanho.LarguraPixelVisualizacao(), tamanho.AlturuaPixelVisualizacao());
        }

        public static int LarguraPixel(this Size tamanho)
        {
            var largura = (tamanho.Width / 2.54) * DPI_RENDERIZAR;
            return (int)largura;
        }

        public static int AlturuaPixel(this Size tamanho)
        {
            var altura = (tamanho.Height / 2.54) * DPI_RENDERIZAR;
            return (int)altura;
        }

        public static int LarguraPixelVisualizacao(this Size tamanho)
        {
            var largura = (tamanho.Width / 2.54) * DPI_VISUALIZACAO;
            return (int)largura;
        }

        public static int AlturuaPixelVisualizacao(this Size tamanho)
        {
            var altura = (tamanho.Height / 2.54) * DPI_VISUALIZACAO;
            return (int)altura;
        }
    }
}
