using Snebur.ServicoArquivo;
using Snebur.Utilidade;

namespace Snebur.Dominio
{
    public static class IImagemExtensao
    {
        /// <summary>
        /// Retornar a url da imagem de visualização, null se não existir
        /// </summary>
        /// <param name="tamanhoImagem"></param>
        /// <returns></returns>
        public static string RetornarUrlImagem(this IImagem imagem, EnumTamanhoImagem tamanhoImagem)
        {
            var urlServicoImagem = ConfiguracaoUtil.UrlServicoImagem;
            return imagem.RetornarUrlImagem(tamanhoImagem,
                                            urlServicoImagem);
        }

        public static string RetornarUrlImagem(this IImagem imagem,
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
    }
}
