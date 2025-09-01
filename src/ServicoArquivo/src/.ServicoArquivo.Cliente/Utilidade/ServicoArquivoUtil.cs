namespace Snebur.FaceBot.Bot;

public class ServicoArquivoUtil
{
    public static T RetornarInstanciaArquivo<T>(string caminhoArquivo) where T : IArquivo
    {

        throw new NotImplementedException();
    }
    public static T RetornarInstanciaArquivo<T>(FileInfo arquivo) where T : IArquivo
    {
        var checksum = ChecksumUtil.RetornarChecksum(arquivo);
        var instancia = Activator.CreateInstance<T>();

        instancia.CaminhoArquivo = arquivo.FullName;
        instancia.NomeArquivo = arquivo.Name;
        instancia.Checksum = checksum;
        instancia.Status = EnumStatusArquivo.Novo;
        instancia.MimeType = MimeTypeUtil.RetornarMimeTypeEnum(arquivo.Extension);
        instancia.TotalBytesLocal = arquivo.Length;

        return instancia;

    }
}

