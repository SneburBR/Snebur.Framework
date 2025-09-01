using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Snebur.Dominio;
using Snebur.Utilidade;

namespace Snebur.Imagens;

public class EncoderUtil
{

    public static BitmapEncoder RetornarEncoder(string caminhoArquivo, int qualidade= ConstantesImagemApresentacao.QUALIDADE_PADRAO)
    {
        return RetornarEncoder(new FileInfo(caminhoArquivo));
    }

    public static BitmapEncoder RetornarEncoder(FileInfo arquivo, int qualidade = ConstantesImagemApresentacao.QUALIDADE_PADRAO)
    {
        var formado = Utilidade.ImagemUtil.RetornarFormatoExtensao(arquivo);
        return RetornarEncoder(formado);
    }

    public static BitmapEncoder RetornarEncoder(EnumFormatoImagem formato, int qualidade = ConstantesImagemApresentacao.QUALIDADE_PADRAO)
    {
        
        switch (formato)
        {
            case EnumFormatoImagem.JPEG:

                var encoder = new JpegBitmapEncoder();
                encoder.QualityLevel = qualidade;
                return encoder;

            case EnumFormatoImagem.BMP:

                return new BmpBitmapEncoder();

            case EnumFormatoImagem.PNG:

                return new BmpBitmapEncoder();

            case EnumFormatoImagem.TIFF:

                return new TiffBitmapEncoder();

            case EnumFormatoImagem.GIF:

                return new GifBitmapEncoder();

            default:

                throw new Erro(String.Format("O formato do arquivo não é suportado {0}", EnumUtil.RetornarDescricao(formato)));

        }
    }

    public static BitmapEncoder RetornarEncoder(BitmapDecoder decoder, int qualidade = ConstantesImagemApresentacao.QUALIDADE_PADRAO)
    {
        if (decoder is JpegBitmapDecoder)
        {
            var encoder = new JpegBitmapEncoder();
            encoder.QualityLevel = qualidade;
            return encoder;
        }

        if (decoder is PngBitmapDecoder)
        {
            var encoder = new PngBitmapEncoder();
            return encoder;
        }

        if (decoder is TiffBitmapDecoder)
        {
            var encoder = new TiffBitmapEncoder();
            encoder.Compression = TiffCompressOption.Lzw;
            return encoder;
        }

        if (decoder is GifBitmapDecoder)
        {
            var encoder = new GifBitmapEncoder();
            return encoder;
        }

        if (decoder is BmpBitmapDecoder)
        {
            var encoder = new BmpBitmapEncoder();
            return encoder;
        }

        throw new Erro(String.Format("Decoder não suportado {0} ", decoder.GetType().Name));
    }

    public static MemoryStream ConverterParaJpeg(string caminhoArquivo)
    {
        var formato = CorFormatoUtil.RetornarFormatoCor(caminhoArquivo);
        var msDestino = new MemoryStream();
        {
            using (var fs = new FileStream(caminhoArquivo, FileMode.Open))
            {

                var decoder = DecoderUtil.RetornarDecoder(fs);
                var frame = decoder.Frames[0];

                var metadata = SistemaUtil.IsWindowsXp ? null : decoder.Frames[0].Metadata as BitmapMetadata;

                var thumbnail = decoder.Frames[0].Thumbnail;

                var corFormato = CorFormatoUtil.RetornarFormatoCor(frame.Format);
                var perfilPadrao = PerfilIccUtil.RetornarPerfilNativoPadrao(corFormato);
                var contexto = decoder.Frames[0].ColorContexts.RetornarPerfilNativo();

                if (contexto == null || contexto.RetornarCorFormato() != corFormato)
                {
                    contexto = PerfilIccUtil.RetornarPerfilNativoPadrao(corFormato);
                }

                var contextos = new ReadOnlyCollection<ColorContext>(new List<ColorContext>() { contexto });

                BitmapSource imagem = frame;

                if (formato == EnumFormatoCor.Indexed)
                {
                    imagem = new FormatConvertedBitmap(frame, PixelFormats.Rgb24, null, 0);
                }

                var encoder = new JpegBitmapEncoder();
                encoder.QualityLevel = 100;
                encoder.Frames.Add(BitmapFrame.Create(imagem, thumbnail, metadata, contextos));
                encoder.Save(msDestino);
                return msDestino;
            }
        }
    }
}
