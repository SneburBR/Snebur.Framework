namespace Snebur.ServicoArquivo;

public partial class ServicoArquivoUtil
{

    public const int NUMERO_ARQUIVOS_PASTA = 10000;

    public const string EXTENCAO_ARQUIVO = ".arquivo";

    public static string RetornarCaminhoCompletoArquivo(string caminhoDiretorioArquivo, long idArquivo)
    {
        return Path.Combine(caminhoDiretorioArquivo, ServicoArquivoUtil.RetornarNomeArquivo(idArquivo, ServicoArquivoUtil.EXTENCAO_ARQUIVO));
    }

    public static string RetornarCaminhoDiretorioArquivo(string caminhoRepositorioArquivo, long idArquivo)
    {
        var caminhoDiretorioArquivo = Path.Combine(caminhoRepositorioArquivo, ServicoArquivoUtil.RetornarDiretorioArquivo(idArquivo));
        DiretorioUtil.CriarDiretorio(caminhoDiretorioArquivo);
        return caminhoDiretorioArquivo;
    }

    public static string RetornarNomeArquivo(long idArquivo)
    {
        return RetornarNomeArquivo(idArquivo, ServicoArquivoUtil.EXTENCAO_ARQUIVO);
    }
    public static string RetornarNomeArquivo(long idArquivo, string extencao)
    {
        return String.Format("{0}.{1}", idArquivo.ToString(), extencao.Replace(".", String.Empty));
    }

    public static string RetornarDiretorioArquivo(long idArquivo)
    {
        string nomePasta = String.Format("{0:000000}", Convert.ToUInt32(idArquivo / ServicoArquivoUtil.NUMERO_ARQUIVOS_PASTA));
        return nomePasta;
    }
}
