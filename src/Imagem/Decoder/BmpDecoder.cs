using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Snebur.Imagem.Decoder
{
    public class BmpDecoder : IDisposable
    {
        private const int POSICAO_TAMANHO = 2;
        private const int POSICAO_LARGURA = 18;
        private const int POSICAO_ALTURA = 23;
        private const int POSICAO_INICIO_PIXELS = 54;

        public int TotalBytes { get; private set; }
        public int Largura { get; private set; }
        public int Altura { get; private set; }
        public short Formato { get; private set; }

        private BinaryReader Leitor;

        public PixelFormat PixalFormat { get; private set; }

        public BitmapPalette Palete { get; private set; }

        private byte[] BytesOrigem { get; }

        public BmpDecoder(BinaryReader leitor)
        {
            this.Leitor = leitor;
            this.Palete = null;

            if (System.Diagnostics.Debugger.IsAttached)
            {
                using (var ms = new MemoryStream())
                {
                    this.Leitor.BaseStream.CopyTo(ms);
                    this.BytesOrigem = ms.ToArray();
                    this.Leitor.BaseStream.Seek(0, SeekOrigin.Begin);
                }
            }
            //var stride = frame.PixelWidth * 4;
            //var len = frame.PixelHeight * stride;
            //this.BytesImagemNativa = new byte[len];
            //frame.CopyPixels(this.BytesImagemNativa, stride, 0);

        }

        private bool IsCabecalhoDefinido;
        private void DefinirCabecalho()
        {
            if (!this.IsCabecalhoDefinido)
            {
                this.Leitor.BaseStream.Position = 0;
                this.Leitor.ReadBytes(2);
                this.TotalBytes = this.Leitor.ReadInt32();

                this.Leitor.ReadBytes(12); // pular para posicao 18 - largura
                this.Largura = this.Leitor.ReadInt32();
                this.Altura = this.Leitor.ReadInt32();
                this.Leitor.ReadBytes(2); // pular para posicao 23 - formato
                this.Formato = this.Leitor.ReadInt16();
                this.PixalFormat = this.RetornarPixelFormato(this.Formato);
                this.Leitor.ReadBytes(24); //25
                this.IsCabecalhoDefinido = true;
            }

        }

        private PixelFormat RetornarPixelFormato(short formato)
        {
            switch (formato)
            {
                case 16:

                    return PixelFormats.Bgr555;

                case 24:

                    return PixelFormats.Bgr24;

                case 32:

                    return PixelFormats.Bgra32;

                default:

                    throw new Exception("Formato do bmp não é suportado");
            }
        }

        public MemoryStream RetornarImagem(int alturaMaxima)
        {
            this.DefinirCabecalho();
            var scalar = alturaMaxima / (double)this.Altura;
            return this.RetornarImagem(scalar);
        }

        public MemoryStream RetornarImagem(double scalar)
        {
            this.DefinirCabecalho();

            var largura = (int)Math.Round(this.Largura * scalar);
            var altura = (int)Math.Round(this.Altura * scalar);
            var stride = largura * 4;

            var pixelsBytes = this.RetornarPixels(scalar);
            var imagem = BitmapSource.Create(largura, altura, 96, 96, this.PixalFormat, this.Palete, pixelsBytes, stride);

            var caminhoDestino = @"c:\temp\destinoxxx.jpg";
            if (File.Exists(caminhoDestino))
            {
                File.Delete(caminhoDestino);
            }

            var ms = new MemoryStream();
            var jpgEncoder = new JpegBitmapEncoder();
            jpgEncoder.Frames.Add(BitmapFrame.Create(imagem));
            jpgEncoder.Save(ms);
            File.WriteAllBytes(caminhoDestino, ms.ToArray());
            return ms;

        }

        private byte[] RetornarPixels(double scalar)
        {
            switch (this.Formato)
            {
                case 24:

                    return this.RetornarPixels24BitsBicubic(scalar);

                default:

                    throw new NotImplementedException();

            }
        }

        private byte[] RetornarPixels24BitRelativoScalar(double scalar)
        {
            var larguraScalar = (int)Math.Round(this.Largura * scalar);
            var alturaScalar = (int)Math.Round(this.Altura * scalar);
            var estradaScalarBgra32 = larguraScalar * 4;
            var totalBytesScalar = (alturaScalar * estradaScalarBgra32);

            var pixelsScalar = new byte[totalBytesScalar];

            var estradaOrigemBgr24 = this.Largura * 3;
            var totalBytesOrigem = (this.Altura * estradaOrigemBgr24);

            var pular = this.Largura % 4;

            for (var linha = alturaScalar - 1; linha >= 0; linha--)
            {

                for (var coluna = 0; coluna < larguraScalar; coluna++)
                {

                    var linhaOrigem = (int)Math.Round(linha / scalar);
                    var coluaOrigem = (int)Math.Round(coluna / scalar);

                    var linhasOffSetInvertida = (this.Altura - linhaOrigem) - 1;
                    var posicaoLinhaOrigem = (linhasOffSetInvertida * pular) + (linhasOffSetInvertida * estradaOrigemBgr24) + POSICAO_INICIO_PIXELS;
                    var posicaoLeitorPixel = posicaoLinhaOrigem + (coluaOrigem * 3);

                    this.Leitor.BaseStream.Seek(posicaoLeitorPixel, SeekOrigin.Begin);

                    var blue = this.Leitor.ReadByte();
                    var green = this.Leitor.ReadByte();
                    var red = this.Leitor.ReadByte();

                    var posicaoScalar = (linha * larguraScalar * 4);
                    var posicaoScalarPixel = posicaoScalar + (coluna * 3);

                    pixelsScalar[posicaoScalarPixel] = blue;
                    pixelsScalar[posicaoScalarPixel + 1] = green;
                    pixelsScalar[posicaoScalarPixel + 2] = red;

                }
                this.Leitor.ReadBytes(pular);
            }
            return pixelsScalar;
        }

        private byte[] RetornarPixels24Bit(double scalar)
        {
            var larguraScalar = (int)Math.Round(this.Largura * scalar);
            var alturaScalar = (int)Math.Round(this.Altura * scalar);
            var strideScalar = larguraScalar * 4;
            var totalBytesScalar = (alturaScalar * strideScalar);

            var pixelsScalar = new byte[totalBytesScalar];

            var strideOrigem = this.Largura * 4;
            var totalBytesOrigem = (this.Altura * strideOrigem);

            //var pixelsOrigem = new byte[totalBytesOrigem];

            var inicioAlpha = this.Largura * 3;
            var inicioAlphaSclar = larguraScalar * 3;

            var pular = this.Largura % 4;

            for (var linha = this.Altura - 1; linha >= 0; linha--)
            {

                for (var coluna = 0; coluna < this.Largura; coluna++)
                {
                    var blue = this.Leitor.ReadByte();
                    var green = this.Leitor.ReadByte();
                    var red = this.Leitor.ReadByte();

                    //var posicaoOrigem = (linha * this.Largura * 4);
                    //var posicaoOrigemPixel = posicaoOrigem + (coluna * 3);
                    //var posicaoOrigemApha = posicaoOrigem + inicioAlpha + linha;

                    //pixelsOrigem[posicaoOrigemPixel] = blue;
                    //pixelsOrigem[posicaoOrigemPixel + 1] = green;
                    //pixelsOrigem[posicaoOrigemPixel + 2] = red;
                    //bytesImagemOrigem[posicaoOrigemApha] = 0;

                    var linhaScalar = (int)(linha * scalar);
                    var colunaScalar = (int)(coluna * scalar);
                    //var diferencaColuna = colunaScalar % 3;

                    var posicaoScalar = (linhaScalar * larguraScalar * 4);
                    var posicaoScalarPixel = posicaoScalar + (colunaScalar * 3);

                    pixelsScalar[posicaoScalarPixel] = blue;
                    pixelsScalar[posicaoScalarPixel + 1] = green;
                    pixelsScalar[posicaoScalarPixel + 2] = red;
                }
                this.Leitor.ReadBytes(pular);
            }
            return pixelsScalar;
        }

        private byte[] RetornarPixels24BitsBicubic(double scalar)
        {
            var larguraScalar = (int)Math.Round(this.Largura * scalar);
            var alturaScalar = (int)Math.Round(this.Altura * scalar);
            var strideScalar = larguraScalar * 4;
            var totalBytesScalar = (alturaScalar * strideScalar);

            var pixelsScalar = new byte[totalBytesScalar];

            var strideOrigem = this.Largura * 4;
            var totalBytesOrigem = (this.Altura * strideOrigem);

            //var pixelsOrigem = new byte[totalBytesOrigem];

            var inicioAlpha = this.Largura * 3;
            var inicioAlphaSclar = larguraScalar * 3;

            var pular = this.Largura % 4;

            for (var linha = this.Altura - 1; linha >= 0; linha--)
            {

                for (var coluna = 0; coluna < this.Largura; coluna++)
                {
                    var azul = this.Leitor.ReadByte();
                    var verde = this.Leitor.ReadByte();
                    var vermelho = this.Leitor.ReadByte();

                    //var posicaoOrigem = (linha * this.Largura * 4);
                    //var posicaoOrigemPixel = posicaoOrigem + (coluna * 3);
                    //var posicaoOrigemApha = posicaoOrigem + inicioAlpha + linha;

                    //pixelsOrigem[posicaoOrigemPixel] = blue;
                    //pixelsOrigem[posicaoOrigemPixel + 1] = green;
                    //pixelsOrigem[posicaoOrigemPixel + 2] = red;
                    //bytesImagemOrigem[posicaoOrigemApha] = 0;

                    var linhaScalar = (int)(linha * scalar);
                    var colunaScalar = (int)(coluna * scalar);
                    //var diferencaColuna = colunaScalar % 3;

                    var posicaoScalar = (linhaScalar * larguraScalar * 4);
                    var posicaoScalarPixel = posicaoScalar + (colunaScalar * 3);

                    var azulAtual = pixelsScalar[posicaoScalarPixel];
                    var verdeAtual = pixelsScalar[posicaoScalarPixel + 1];
                    var vermelhoAtual = pixelsScalar[posicaoScalarPixel + 1];

                    var azulMesclado = this.RetornarValorPixel(azulAtual, azul);
                    var verdeMesclado = this.RetornarValorPixel(verdeAtual, verde);
                    var vermelhoMesclado = this.RetornarValorPixel(vermelhoAtual, vermelho);

                    pixelsScalar[posicaoScalarPixel] = azulMesclado;
                    pixelsScalar[posicaoScalarPixel + 1] = verdeMesclado;
                    pixelsScalar[posicaoScalarPixel + 2] = vermelhoMesclado;
                }
                this.Leitor.ReadBytes(pular);
            }
            return pixelsScalar;
        }

        private byte RetornarValorPixel(byte valorAtual, byte novoValor)
        {
            if (valorAtual > 0 && valorAtual != novoValor)
            {
                return Convert.ToByte(Math.Round((valorAtual + novoValor) / 2D));
            }
            return novoValor;

        }

        public void Dispose()
        {

        }

    }
}
