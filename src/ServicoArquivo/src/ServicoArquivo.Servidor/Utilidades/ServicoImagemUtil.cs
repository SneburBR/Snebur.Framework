namespace Snebur.ServicoArquivo;

public class ServicoImagemUtil
{
    #region  Métodos

    public static string RetornarCaminhoDiretorioImagem(string caminhoRepositorioImagens, long idImagem)
    {
        var caminhoImagem = Path.Combine(caminhoRepositorioImagens, ServicoArquivoUtil.RetornarDiretorioArquivo(idImagem));
        DiretorioUtil.CriarDiretorio(caminhoImagem);
        return caminhoImagem;
    }

    public static string RetornarExtensaoImagem(EnumTamanhoImagem tamanhoImagem)
    {
        switch (tamanhoImagem)
        {
            case EnumTamanhoImagem.Miniatura:
                return "miniatura";
            case EnumTamanhoImagem.Pequena:
                return "pequena";
            case EnumTamanhoImagem.Media:
                return "media";
            case EnumTamanhoImagem.Grande:
                return "grande";
            case EnumTamanhoImagem.Impressao:
                return "origem";
            default:
                throw new NotSupportedException(String.Format("O tamanho de imagem {0} não é suportado.", tamanhoImagem.ToString()));
        }
    }

    public static object RetornarCaminhoCompletoImagem(string caminhoImagem, long idImagem, EnumTamanhoImagem tamanhoImagem)
    {
        return Path.Combine(caminhoImagem, String.Format("{0}.{1}", idImagem, ServicoImagemUtil.RetornarExtensaoImagem(tamanhoImagem)));
    }
    //public static long RetornarIdImagem(HttpContext context)
    //{
    //    var idImagem = Convert.ToInt64(context.Request.QueryString[ConstantesServicoArquivo.ID_ARQUIVO]);
    //    if (!(idImagem > 0))
    //    {
    //        throw new Exception(string.Format("Parâmetro '{0}' não suportado.", ConstantesServicoArquivo.ID_ARQUIVO));
    //    }
    //    return idImagem;
    //}

    //public static EnumTamanhoImagem RetornarTamanhoImagem(HttpContext context)
    //{
    //    var parametroTamanhoImagem = context.Request.QueryString[ConstantesServicoImagem.TAMANHO_IMAGEM];
    //    var tamanhoImagem = (EnumTamanhoImagem)System.Enum.Parse(typeof(EnumTamanhoImagem), parametroTamanhoImagem);

    //    if(!Enum.IsDefined(typeof(EnumTamanhoImagem), tamanhoImagem))
    //    {
    //        throw new Exception(string.Format("Parâmetro '{0}' não suportado.", ConstantesServicoImagem.TAMANHO_IMAGEM));
    //    }
    //    return tamanhoImagem;
    //}

    #endregion
}