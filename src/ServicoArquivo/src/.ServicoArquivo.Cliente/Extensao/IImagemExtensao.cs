using Snebur.Serializacao;
using Snebur.ServicoArquivo;

namespace Snebur.Dominio;

public static class IImagemExtensao
{
    /// <summary>
    /// Retornar a url da imagem de visualização, null se não existir
    /// </summary>
    /// <param name="tamanhoImagem"></param>
    /// <returns></returns>
    public static string? RetornarUrlImagem(this IImagem imagem, EnumTamanhoImagem tamanhoImagem)
    {
        var urlServicoImagem = ConfiguracaoUtil.UrlServicoImagem;
        Guard.NotNullOrWhiteSpace(urlServicoImagem);
        return imagem.RetornarUrlImagem(tamanhoImagem,
                                        urlServicoImagem);
    }

    public static string? RetornarUrlImagem(this IImagem imagem,
                                          EnumTamanhoImagem tamanhoImagem,
                                          string urlServicoImagem)
    {
        if (imagem.IsExisteImagem(tamanhoImagem))
        {
            return ServicoImagemClienteUtil.RetornarUrlVisualizarImagem(urlServicoImagem,
                                                                       imagem,
                                                                       tamanhoImagem);
        }
        return null;
    }

    public static bool IsExisteImagem(this IImagem imagem, EnumTamanhoImagem tamanhoImagem)
    {
        return ImagemUtilEx.IsExisteImagem(imagem, tamanhoImagem);
    }

    public static void SetIsExisteImagem(this IImagem imagem,
                                         EnumTamanhoImagem tamanhoImagem,
                                         bool isExiste)
    {
        (imagem as IBaseDominioControladorPropriedade)?.DestivarControladorPropriedadeAlterada();
        switch (tamanhoImagem)
        {
            case EnumTamanhoImagem.Miniatura:
                imagem.IsExisteMiniatura = isExiste;
                break;
            case EnumTamanhoImagem.Pequena:
                imagem.IsExistePequena = isExiste;
                break;
            case EnumTamanhoImagem.Media:
                imagem.IsExisteMedia = isExiste;
                break;
            case EnumTamanhoImagem.Grande:
                imagem.IsExisteGrande = isExiste;
                break;
            case EnumTamanhoImagem.Impressao:
                imagem.IsExisteArquivo = isExiste;
                break;
            default:
                throw new Erro("Tamanho da imagem não é suportado");
        }

        (imagem as IBaseDominioControladorPropriedade)?.AtivarControladorPropriedadeAlterada();

    }
}
