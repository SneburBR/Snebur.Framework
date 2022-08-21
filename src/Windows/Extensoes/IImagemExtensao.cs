using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur.Dominio;
using Snebur.Utilidade;

namespace Snebur.Windows
{
    public static class IImagemExtensao
    {
        public static string RetornarUrlImagem(this IImagem imagem, EnumTamanhoImagem tamanhoImagem)
        {
            var urlServicoImagem = AplicacaoSnebur.Atual.UrlServicoImagem;
            var urlImagemImpressao = Snebur.ServicoArquivo.ServicoImagemClienteUtil.RetornarUrlVisualizarImagem(urlServicoImagem,
                                                                                                                  imagem,
                                                                                                                  tamanhoImagem);
            return urlImagemImpressao;
        }

        public static MemoryStream RetornarStreamImagem(this IImagem imagem, 
                                                        EnumTamanhoImagem tamanhoImagem)
        {
            var urlImagem = imagem.RetornarUrlImagem(tamanhoImagem);
            return HttpUtil.RetornarMemoryStream(urlImagem);
        }

        public static Task<MemoryStream> RetornarStreamImagemAwait(this IImagem imagem,
                                                             EnumTamanhoImagem tamanhoImagem)
        {
            return Task.Factory.StartNew(() =>
            {
                return RetornarStreamImagem(imagem, tamanhoImagem);
            });
        }
    }
}
