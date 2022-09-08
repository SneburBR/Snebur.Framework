using Snebur.Dominio;
using Snebur.Utilidade;
using System;
using System.IO;

namespace Snebur.ServicoArquivo.Cliente
{
    public class BaixarImagemUtil
    {
        public static MemoryStream RetornarStreamImpressao(IImagem imagem)
        {
            return RetornarStream(imagem, EnumTamanhoImagem.Impressao);
        }

        public static MemoryStream RetornarStream(IImagem imagem, EnumTamanhoImagem tamanhoImagem)
        {
            return RetornarStream(AplicacaoSnebur.Atual.UrlServicoArquivo, imagem, tamanhoImagem);
        }

        public static MemoryStream RetornarStreamImpressao(string urlServicoImagem, IImagem imagem)
        {
            return RetornarStream(urlServicoImagem, imagem, EnumTamanhoImagem.Impressao);
        }

        public static MemoryStream RetornarStream(string urlServicoImagem, IImagem imagem, EnumTamanhoImagem tamanhoImagem)
        {
            var urlImagem = ServicoImagemClienteUtil.RetornarUrlVisualizarImagem(urlServicoImagem, imagem, tamanhoImagem);
            return HttpUtil.RetornarMemoryStream(urlImagem);
        }

        public static MemoryStream RetornarStream(string urlServicoImagem, long idImagem, string nomeTipoImagem, EnumTamanhoImagem tamanhoImagem)
        {
            return BaixarImagemUtil.RetornarStream(urlServicoImagem, idImagem, nomeTipoImagem, tamanhoImagem, String.Empty);
        }

        public static MemoryStream RetornarStream(string urlServicoImagem, long idImagem, string nomeTipoImagem, EnumTamanhoImagem tamanhoImagem, string identificadorCache)
        {
            var urlImagem = ServicoImagemClienteUtil.RetornarUrlVisualizarImagem(urlServicoImagem, idImagem, nomeTipoImagem, tamanhoImagem, identificadorCache);
            return HttpUtil.RetornarMemoryStream(urlImagem);
        }

    }
}
