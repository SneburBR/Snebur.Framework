using Snebur.Dominio;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;

namespace Snebur.ServicoArquivo
{
    public partial class ServicoArquivoClienteUtil
    {

        public static string RetornaUrlBaixarVersao(string urlServicoArquivo, IArquivo arquivo)
        {
            return RetornaUrlBaixarVersao(urlServicoArquivo, arquivo, null);
        }

        public static string RetornaUrlBaixarVersao(string urlServicoArquivo, IArquivo arquivo, string identificadorCache)
        {
            var nomeArquivo = ConstantesServicoArquivo.NOME_ARQUIVO_BAIXAR_VERSAO;
            var urlBaixarArquivo = UriUtil.CombinarCaminhos(urlServicoArquivo, nomeArquivo);
            var nomeTipoArquivo = arquivo.GetType().Name;

            var parametros = new Dictionary<string, string>
            {
                { ConstantesServicoArquivo.ID_ARQUIVO, arquivo.Id.ToString() },
                { ConstantesServicoArquivo.NOME_TIPO_ARQUIVO, nomeTipoArquivo }

            };

            if (!String.IsNullOrEmpty(identificadorCache))
            {
                parametros.Add(ConstantesServicoArquivo.IDENTIFICADOR_CACHE, identificadorCache);
            }
            return UriUtil.RetornarURL(urlBaixarArquivo, parametros);
        }
        public static string RetornarEnderecoBaixarArquivo(string urlServicoArquivo)
        {
            var nomeArquivo = ConstantesServicoArquivo.NOME_ARQUIVO_BAIXAR_ARQUIVO + "?" + Guid.NewGuid().ToString();
            return UriUtil.CombinarCaminhos(urlServicoArquivo, nomeArquivo);
        }

        public static string RetornarEnderecoEnviarArquivo(string urlServicoArquivo)
        {
            var nomeArquivo = ConstantesServicoArquivo.NOME_ARQUIVO_ENVIAR_ARQUIVO + "?" + Guid.NewGuid().ToString();
            return UriUtil.CombinarCaminhos(urlServicoArquivo, nomeArquivo);
        }
    }
}
