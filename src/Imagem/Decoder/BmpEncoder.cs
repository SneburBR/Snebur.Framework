using System.IO;
using System.Text;
using System.Windows.Media.Imaging;
using Snebur.Utilidade;

namespace Snebur.Imagens;

public class BmpEncoder
{
    private const int RESERVED = 0;
    private const short LLANE = 1;
    private const short BITS_PER_PIXELS = 24;
    private const int COMPRESS = 0;
    private const int HR = 0;
    private const int VR = 0;
    private const int COLORS = 0;
    private const int IMPORTANT_COLORS = 0;
    private const int TAMANHO_CABECALHO = 54;
    private const int HEADER_INFO_SIZE = 40;
    private const string FLAB_BM = "BM";

    //private Stream StreamOrigem { get; set; }
    //private Stream streamDestino { get; set; }

    //private byte[] pixelsOrigem { get; set; }

    //Obsoleto
    //private byte[] BytesDestino { get; set; }

    //private BinaryReader ReaderOrigem { get; set; }
    //private BinaryWriter writerDestino { get; set; }

    private bool BufferAlpha { get; set; } = true;
    private int Largura { get; set; }
    private int Altura { get; set; }
    private int TotalBytesExtraPorLinha { get; set; }
    private int TamanhoFrameRgb { get; set; }
    private int TamanhoArquivo { get; set; }
    private int Posicao { get; set; }

    public BmpEncoder()
    {

    }

    private void AtriburValoresPropriedadesCabecalho(int largura, int altura)
    {
        this.BufferAlpha = true;
        this.Largura = largura;
        this.Altura = altura;
        this.TotalBytesExtraPorLinha = this.Largura % 4;
        this.TamanhoFrameRgb = this.Altura * (3 * this.Largura + this.TotalBytesExtraPorLinha);
        this.TamanhoArquivo = (this.TamanhoFrameRgb + TAMANHO_CABECALHO);
    }

    public void Salvar(BitmapSource bitmapSource, string caminhoDestino)
    {
        ArquivoUtil.DeletarArquivo(caminhoDestino);
        var streamDestino = new FileStream(caminhoDestino, FileMode.Create, FileAccess.Write, FileShare.Write);

        this.SalvarPartes(bitmapSource, streamDestino, 10);

        streamDestino.Dispose();
    }

    public void SalvarPartes(BitmapSource bitmapSource, Stream streamDestino, int totalPartes)
    {
        var writerDestino = new BinaryWriter(streamDestino);

        var pixels = bitmapSource.GetPixels(EnumPixelFormato.Rgba);

        var largura = bitmapSource.PixelWidth;
        var altura = bitmapSource.PixelHeight;

        this.AtriburValoresPropriedadesCabecalho(bitmapSource.PixelWidth, bitmapSource.PixelHeight);
        this.AjustarTamanhoStream(streamDestino);
        //var alturaParte = bitmapSource.PixelHeight;
        //var pixelsParte = pixels;
        var bytesPorLinha = largura * 4;

        var alturaParte = altura / totalPartes;
        for (var i = 0; i < totalPartes; i++)
        {
            var parteAtual = i + 1;
            var offset = (i * (bytesPorLinha * alturaParte));

            if (parteAtual == totalPartes)
            {
                alturaParte += altura % totalPartes;
            }
            var len = bytesPorLinha * alturaParte;
            var pixelsParte = pixels.Skip(offset).Take(len).ToArray();

            this.SalvarPixels(writerDestino, largura, altura, pixelsParte, alturaParte, parteAtual, totalPartes);
            //break;
        }
    }
    //public void Salvar(BitmapSource bitmapSource, Stream streamDestino)
    //{
    //    var pixels = bitmapSource.GetPixels(EnumPixelFormato.Rgba);

    //    var largura = bitmapSource.PixelWidth;
    //    var altura = bitmapSource.PixelHeight;
    //    var alturaParte = bitmapSource.PixelHeight;
    //    var pixelsParte = pixels;
    //    var parteAtual = 1;
    //    var totalPartes = 1;

    //    this.Salvar(streamDestino, largura, altura, pixelsParte, alturaParte, parteAtual, totalPartes);
    //}

    //private void Salvar(BinaryWriter streamDestino, int largura, int altura, byte[] pixelsParte, int alturaParte, int parteAtual, int totalPartes)
    //{

    //    var writerDestino = new BinaryWriter(streamDestino);
    //    this.SalvarPixels(writerDestino, pixelsParte, alturaParte, parteAtual, totalPartes);
    //}

    public void SalvarPixels(BinaryWriter writerDestino, int largura, int altura, byte[] pixelsParte, int alturaParte, int parteAtual, int totalPartes)
    {
        this.AtriburValoresPropriedadesCabecalho(largura, altura);

        //this.SalvarCabecalho(writerDestino);
        if (parteAtual == 1)
        {
            this.SalvarCabecalho(writerDestino);
        }

        var bytesPorPixel = (this.BufferAlpha) ? 4 : 3;
        var bytesLinhaDestino = 3 * this.Largura + this.TotalBytesExtraPorLinha;
        var strideOrigem = this.Largura * bytesPorPixel;

        var linhaOrigem = 0;

        //for (var linha = this.Height - 1; linha >= 0; linha--)

        var tamanhoBuffer = alturaParte * bytesLinhaDestino;
        var bufferParte = new byte[tamanhoBuffer];

        for (var linha = 0; linha < alturaParte; linha++)
        {
            linhaOrigem = (alturaParte - (linha + 1));
            for (var coluna = 0; coluna < this.Largura; coluna++)
            {

                var posicaoOrigem = linhaOrigem * strideOrigem + (coluna * bytesPorPixel);
                var posicaoDestino = linha * bytesLinhaDestino + coluna * 3;

                //writerDestino.BaseStream.Position = posicaoDestino;
                var red = pixelsParte[posicaoOrigem++];
                var green = pixelsParte[posicaoOrigem++];
                var blue = pixelsParte[posicaoOrigem++];

                //byte[] pixel = new byte[3] { blue, green, red };
                //writerDestino.Write(pixel);
                //this.StreamDestino.Seek(p, SeekOrigin.Begin);
                //this.WriterDestino.Write(redr);
                //this.WriterDestino.Write(green);
                //this.WriterDestino.Write(blue);

                bufferParte[posicaoDestino] = blue;
                bufferParte[posicaoDestino + 1] = green;
                bufferParte[posicaoDestino + 2] = red;

                //if (this.BufferAlpha)
                //{
                //    i++;
                //}
            }

            //if (this.TotalBytesExtraPorLinha > 0)
            //{
            //    var bytesExtras = new byte[this.TotalBytesExtraPorLinha];
            //    writerDestino.Write(bytesExtras);
            //    //    var fillOffset = this.pos + y * rowBytes + this.Width * 3;
            //    //    BytesDestino.fill(0, fillOffset, fillOffset + this.ExtraBytes);
            //}
        }

        int offsetStremOrigem = 0;
        if (parteAtual == totalPartes)
        {
            offsetStremOrigem = TAMANHO_CABECALHO;
        }
        else
        {
            var diferenca = (totalPartes - parteAtual);

            offsetStremOrigem = TAMANHO_CABECALHO + (diferenca * (alturaParte * bytesLinhaDestino));
        }

        writerDestino.BaseStream.Seek(offsetStremOrigem, SeekOrigin.Begin);
        writerDestino.Write(bufferParte);
        //var frame = this.BytesDestino.Skip(54).ToArray();
        //this.WriterDestino.Write(frame, 0, frame.Count());
    }

    private void SalvarCabecalho(BinaryWriter writerDestino)
    {
        //this.Largura = largura;
        //this.Altura = altura;
        //this.TotalBytesExtraPorLinha = this.Largura % 4;
        //this.TamanhoFrameRgb = this.Altura * (3 * this.Largura + this.TotalBytesExtraPorLinha);
        //this.TamanhoArquivo = (this.TamanhoFrameRgb + TAMANHO_CABECALHO);

        writerDestino.BaseStream.Seek(0, SeekOrigin.Begin);

        writerDestino.Write(Encoding.UTF8.GetBytes(BmpEncoder.FLAB_BM.ToCharArray()));

        writerDestino.Write(this.TamanhoArquivo);

        writerDestino.Write(BmpEncoder.RESERVED);

        writerDestino.Write(TAMANHO_CABECALHO);

        writerDestino.Write(HEADER_INFO_SIZE);

        writerDestino.Write(this.Largura);

        writerDestino.Write(this.Altura);

        writerDestino.Write(BmpEncoder.LLANE);

        writerDestino.Write(BmpEncoder.BITS_PER_PIXELS);

        writerDestino.Write(BmpEncoder.COMPRESS);

        writerDestino.Write(this.TamanhoFrameRgb);

        writerDestino.Write(BmpEncoder.HR);

        writerDestino.Write(BmpEncoder.VR);

        writerDestino.Write(BmpEncoder.COLORS);

        writerDestino.Write(BmpEncoder.IMPORTANT_COLORS);
    }

    private void AjustarTamanhoStream(Stream streamDestino)
    {
        if (streamDestino is FileStream fsDestino)
        {
            if (streamDestino.Length < this.TamanhoArquivo)
            {
                streamDestino.Seek(this.TamanhoArquivo - 1, SeekOrigin.Begin);
                streamDestino.Write(new byte[] { 0 }, 0, 1);
                fsDestino.Flush(true);
            }
        }
    }

    //private void SalvarBytesDestinoObsoleto()
    //{
    //    this.BytesDestino = new byte[TAMANHO_CABECALHO + this.TamanhoFrameRgb];

    //    BytesDestino.Write(BmpEncoder.FLAB_BM, this.Posicao, 2);
    //    this.Posicao += 2;
    //    BytesDestino.WriteUInt32LE(this.TamanhoArquivo, this.Posicao);
    //    this.Posicao += 4;
    //    BytesDestino.WriteUInt32LE(BmpEncoder.RESERVED, this.Posicao);
    //    this.Posicao += 4;
    //    BytesDestino.WriteUInt32LE(TAMANHO_CABECALHO, this.Posicao);
    //    this.Posicao += 4;
    //    BytesDestino.WriteUInt32LE(HEADER_INFO_SIZE, this.Posicao);
    //    this.Posicao += 4;
    //    BytesDestino.WriteUInt32LE(this.Largura, this.Posicao);
    //    this.Posicao += 4;
    //    BytesDestino.WriteUInt32LE(this.Altura, this.Posicao);
    //    this.Posicao += 4;
    //    BytesDestino.WriteUInt16LE(BmpEncoder.LLANE, this.Posicao);
    //    this.Posicao += 2;
    //    BytesDestino.WriteUInt16LE(BmpEncoder.BITS_PER_PIXELS, this.Posicao);
    //    this.Posicao += 2;
    //    BytesDestino.WriteUInt32LE(BmpEncoder.COMPRESS, this.Posicao);
    //    this.Posicao += 4;
    //    BytesDestino.WriteUInt32LE(this.TamanhoFrameRgb, this.Posicao);
    //    this.Posicao += 4;
    //    BytesDestino.WriteUInt32LE(BmpEncoder.HR, this.Posicao);
    //    this.Posicao += 4;
    //    BytesDestino.WriteUInt32LE(BmpEncoder.VR, this.Posicao);
    //    this.Posicao += 4;
    //    BytesDestino.WriteUInt32LE(BmpEncoder.COLORS, this.Posicao);
    //    this.Posicao += 4;
    //    BytesDestino.WriteUInt32LE(BmpEncoder.IMPORTANT_COLORS, this.Posicao);
    //    this.Posicao += 4;

    //    var i = 0;
    //    var rowBytes = 3 * this.Largura + this.TotalBytesExtraPorLinha;

    //    for (var y = this.Altura - 1; y >= 0; y--)
    //    {
    //        for (var x = 0; x < this.Largura; x++)
    //        {
    //            var p = this.Posicao + y * rowBytes + x * 3;
    //            BytesDestino[p + 2] = pixelsOrigem[i++];//r
    //            BytesDestino[p + 1] = pixelsOrigem[i++];//g
    //            BytesDestino[p] = pixelsOrigem[i++];//b
    //            if (this.BufferAlpha)
    //            {
    //                i++;
    //            }
    //        }
    //        //if (this.ExtraBytes > 0)
    //        //{
    //        //    var fillOffset = this.pos + y * rowBytes + this.Width * 3;
    //        //    BytesDestino.fill(0, fillOffset, fillOffset + this.ExtraBytes);
    //        //}
    //    }
    //    this.streamDestino.Write(BytesDestino, 0, BytesDestino.Length);
    //}

}

public static class Extensoes
{
    public static void Write(this byte[] origem, string conteudo, int offset)
    {
        Extensoes.Write(origem, conteudo, offset, conteudo.Length);
    }

    public static void Write(this byte[] origem, string conteudo, int offset, int length)
    {
        Extensoes.Write(origem, conteudo, offset, length, Encoding.UTF8);
    }

    public static void Write(this byte[] origem, string conteudo, int offset, int length, Encoding encoding)
    {
        var bytes = encoding.GetBytes(conteudo.ToCharArray(), 0, length);
        Extensoes.WriteBytes(origem, bytes, offset);
    }

    public static void WriteUInt32LE(this byte[] origem, int valor, int offset)
    {
        WriteUInt32LE(origem, (uint)valor, offset);
    }

    public static void WriteUInt32LE(this byte[] origem, uint valor, int offset)
    {
        var bytes = BitConverter.GetBytes(valor);
        Extensoes.WriteBytes(origem, bytes, offset);
    }

    public static void WriteUInt16LE(this byte[] origem, short valor, int offset)
    {
        Extensoes.WriteUInt16LE(origem, (ushort)valor, offset);
    }

    public static void WriteUInt16LE(this byte[] origem, ushort valor, int offset)
    {
        var bytes = BitConverter.GetBytes(valor);
        Extensoes.WriteBytes(origem, bytes, offset);

    }

    public static void fill(this byte[] origem, byte valor, int offset, int fim)
    {
        var bytes = new byte[fim - offset];
        Extensoes.WriteBytes(origem, bytes, offset);
    }

    public static void WriteBytes(this byte[] origem, byte[] bytes, int offset)
    {
        for (var i = 0; i < bytes.Length; i++)
        {
            origem[offset + i] = bytes[i];
        }
    }

    //public static byte[] GetPixels(this BitmapSource origem, EnumPixelFormato formato)
    //{
    //    var stride = origem.PixelWidth * 4;
    //    var rgb24 = new FormatConvertedBitmap(origem, PixelFormats.Rgb24, null, 0);
    //    var pixelsRgb24 = new byte[origem.PixelHeight * stride];
    //    rgb24.CopyPixels(pixelsRgb24, stride, 0);

    //    if (formato == EnumPixelFormato.Rgb24)
    //    {
    //        return pixelsRgb24;
    //    }

    //    if (formato == EnumPixelFormato.Rgba)
    //    {
    //        var pixelsRgba = new byte[origem.PixelHeight * stride];
    //        for (var linha = 0; linha < origem.PixelHeight; linha++)
    //        {
    //            for (var coluna = 0; coluna < origem.PixelWidth; coluna++)
    //            {
    //                var posicaoRgb24 = (linha * stride) + (coluna * 3);
    //                var posicaoRgba = (linha * stride) + (coluna * 4);

    //                var r = pixelsRgb24[posicaoRgb24];
    //                var g = pixelsRgb24[posicaoRgb24 + 1];
    //                var b = pixelsRgb24[posicaoRgb24 + 2];

    //                pixelsRgba[posicaoRgba] = r;
    //                pixelsRgba[posicaoRgba + 1] = g;
    //                pixelsRgba[posicaoRgba + 2] = b;
    //            }

    //        }
    //        return pixelsRgba;
    //    }

    //    throw new NotSupportedException();

    //}
}

//public enum EnumPixelFormato
//{
//    Rgba,
//    Rgb24,
//}

//private void SalvarInterno()
//{
//    this.pos = 0;

//    this.tempBuffer.Write(this.flag);
//    this.pos += 2;
//    this.tempBuffer.Write(this.fileSize);
//    this.pos += 4;
//    this.tempBuffer.Write(this.reserved);
//    this.pos += 4;
//    this.tempBuffer.Write(this.offset); this.pos += 4;

//    this.tempBuffer.Write(this.headerInfoSize);
//    this.pos += 4;
//    this.tempBuffer.Write(this.width);
//    this.pos += 4;
//    this.tempBuffer.Write(this.height);
//    this.pos += 4;
//    this.tempBuffer.Write(this.planes);
//    this.pos += 2;
//    this.tempBuffer.Write(this.bitPP);
//    this.pos += 2;
//    this.tempBuffer.Write(this.compress);
//    this.pos += 4;
//    this.tempBuffer.Write(this.rgbSize);
//    this.pos += 4;
//    this.tempBuffer.Write(this.hr);
//    this.pos += 4;
//    this.tempBuffer.Write(this.vr);
//    this.pos += 4;
//    this.tempBuffer.Write(this.colors);
//    this.pos += 4;
//    this.tempBuffer.Write(this.importantColors);
//    this.pos += 4;

//    var i = 0;
//    var rowBytes = 3 * this.width + this.extraBytes;

//    for (var y = this.height - 1; y >= 0; y--)
//    {
//        for (var x = 0; x < this.width; x++)
//        {
//            var p = this.pos + y * rowBytes + x * 3;
//            tempBuffer.Write(p);

//            tempBuffer[p + 2] = this.buffer[i++];//r
//            tempBuffer[p + 1] = this.buffer[i++];//g
//            tempBuffer[p] = this.buffer[i++];//b

//            if (this.bufferAlpha)
//            {
//                i++;
//            }
//        }
//        if (this.extraBytes > 0)
//        {
//            var fillOffset = this.pos + y * rowBytes + this.width * 3;
//            tempBuffer.fill(0, fillOffset, fillOffset + this.extraBytes);
//        }
//    }

//}