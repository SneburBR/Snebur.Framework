using System.IO;
using System.Windows.Media.Imaging;
using Snebur.Dominio;

namespace Snebur.Imagens;

public class ConverterCorFormatoImagemUtil
{
    public static MemoryStream ConverterFormatado(string caminhoArquivoFormatoOrigem, Stream streamImagem, EnumFormatoImagem formato, int qualidade = ConstantesImagemApresentacao.QUALIDADE_PADRAO)
    {
        var encoder = EncoderUtil.RetornarEncoder(formato, qualidade);
        streamImagem.Position = 0;
        encoder.Frames.Add(BitmapFrame.Create(streamImagem));
        var ms = new MemoryStream();
        encoder.Save(ms);
        return ms;
    }
}
