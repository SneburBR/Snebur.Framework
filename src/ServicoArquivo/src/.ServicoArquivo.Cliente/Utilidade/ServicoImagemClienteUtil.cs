using Snebur.Dominio;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;

namespace Snebur.ServicoArquivo
{
    public partial class ServicoImagemClienteUtil
    {
        public static string RetornarEnderecoBaixarImagem(string enderecoServicoImagem)
        {
            var nomeArquivo = ConstantesServicoImagem.NOME_ARQUIVO_BAIXAR_IMAGEM + "?" + Guid.NewGuid().ToString();
            return UriUtil.CombinarCaminhos(enderecoServicoImagem, nomeArquivo);
        }

        public static string RetornarEnderecoEnviarImagem(string enderecoServicoImagem)
        {
            if (String.IsNullOrWhiteSpace(enderecoServicoImagem))
            {
                throw new ArgumentNullException($"O argumento {nameof(enderecoServicoImagem)} não foi definido");
            }

            var nomeArquivo = ConstantesServicoImagem.NOME_ARQUIVO_ENVIAR_IMAGEM + "?" + Guid.NewGuid().ToString();
            return UriUtil.CombinarCaminhos(enderecoServicoImagem, nomeArquivo);
        }

        public static string RetornarUrlVisualizarImagem(string urlServicoImagem, IImagem imagem, EnumTamanhoImagem tamanhoImagem)
        {
            return RetornarUrlVisualizarImagem(urlServicoImagem, imagem.Id, imagem.GetType().Name, tamanhoImagem, String.Empty);
        }
        public static string RetornarUrlVisualizarImagem(string urlServicoImagem, IImagem imagem, EnumTamanhoImagem tamanhoImagem, string identificadorCache)
        {
            return RetornarUrlVisualizarImagem(urlServicoImagem, imagem.Id, imagem.GetType().Name, tamanhoImagem, identificadorCache);
        }

        public static string RetornarUrlVisualizarImagem(string urlServicoImagem, long idImagem, string nomeTipoImagem, EnumTamanhoImagem tamanhoImagem, string identificadorCache)
        {
            var urlVisualizar = UriUtil.CombinarCaminhos(urlServicoImagem, ConstantesServicoImagem.NOME_ARQUIVO_VIZUALIZAR_IMAGEM);
            var parametros = new Dictionary<string, string>
            {
                { ConstantesServicoImagem.ID_IMAGEM, idImagem.ToString() },
                { ConstantesServicoImagem.TAMANHO_IMAGEM, Convert.ToInt32(tamanhoImagem).ToString() },
                { ConstantesServicoArquivo.NOME_TIPO_ARQUIVO, nomeTipoImagem }

            };
            if (!String.IsNullOrEmpty(identificadorCache))
            {
                parametros.Add(ConstantesServicoArquivo.IDENTIFICADOR_CACHE, identificadorCache);
            }
            var url = UriUtil.RetornarURL(urlVisualizar, parametros);
            return $"{url}&{EnumUtil.RetornarDescricao(tamanhoImagem)}";
        }



    }
}