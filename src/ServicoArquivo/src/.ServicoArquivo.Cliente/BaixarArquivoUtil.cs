﻿using Snebur.Dominio;
using Snebur.Seguranca;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;

namespace Snebur.ServicoArquivo.Cliente
{

    public class BaixarArquivoUtil
    {
        public static TimeSpan TIMEOUT_PADRAO { get; } = TimeSpan.FromMinutes(5);

       
        public static MemoryStream RetornarStream(string urlServico,
                                                  IArquivo arquivo,
                                                  Guid identificadorSessao,
                                                  string identificadorProprietario,
                                                  CredencialUsuario credencialUsuario)
        {
            return RetornarStream(urlServico,
                                  arquivo,
                                  identificadorSessao,
                                  identificadorProprietario,
                                  credencialUsuario,
                                  TIMEOUT_PADRAO);
        }

        public static MemoryStream RetornarStream(string urlServico,
                                                  IArquivo arquivo,
                                                  Guid identificadorSessao,
                                                  string identificadorProprietario,
                                                  CredencialUsuario credencialUsuario,
                                                  TimeSpan timeout)
        {
            var nomeTipoArquivo = arquivo.GetType().Name;
            var nomeTipoQualificado = arquivo.GetType().AssemblyQualifiedName;

            return RetornarStream(urlServico,
                                  arquivo.Id,
                                  nomeTipoArquivo,
                                  nomeTipoQualificado,
                                  identificadorSessao,
                                  identificadorProprietario,
                                  credencialUsuario,
                                  timeout);

        }

        public static MemoryStream RetornarStream(string urlServico, long idArquivo, string nomeTipoArquivo, string nomeTipoQualificado)
        {
            //var identificadorSessao = AplicacaoSnebur.Atual.IdentificadorSessaoUsuario;

            var informacao = AplicacaoSnebur.Atual.InformacaoSessaoUsuario;
            var identificadorProprietario = AplicacaoSnebur.Atual.IdentificadorProprietario;
            var credencialUsuario = AplicacaoSnebur.Atual.CredencialUsuario;

            return RetornarStream(urlServico,
                                  idArquivo,
                                  nomeTipoArquivo,
                                  nomeTipoQualificado,
                                  informacao.IdentificadorSessaoUsuario,
                                  identificadorProprietario,
                                  credencialUsuario,
                                  TIMEOUT_PADRAO);
        }
        public static MemoryStream RetornarStream(string urlServico,
                                                  long idArquivo,
                                                  string nomeTipoArquivo,
                                                  string nomeTipoQualificado,
                                                  Guid identificadorSessao,
                                                  string identificadorProprietario,
                                                  CredencialUsuario credencialUsuario)
        {
            return RetornarStream(urlServico,
                                  idArquivo,
                                  nomeTipoArquivo,
                                  nomeTipoQualificado,
                                  identificadorSessao,
                                  identificadorProprietario,
                                  credencialUsuario,
                                  TIMEOUT_PADRAO);
        }

        public static MemoryStream RetornarStream(string urlServico,
                                                    long idArquivo,
                                                    string nomeTipoArquivo,
                                                    string nomeTipoQualificado,
                                                    Guid identificadorSessao,
                                                    string identificadorProprietario,
                                                    CredencialUsuario credencialUsuario,
                                                    TimeSpan timeout)
        {
            var nomeAssembly = AplicacaoSnebur.Atual.NomeAplicacao;
            var parametros = new Dictionary<string, string>();
            var token = Token.RetornarToken();


            parametros.Add(ConstantesServicoArquivo.ID_ARQUIVO, idArquivo.ToString());
            parametros.Add(ConstantesServicoArquivo.ASEMMBLY_QUALIFIED_NAME, nomeTipoQualificado);
            parametros.Add(ConstantesServicoArquivo.NOME_TIPO_ARQUIVO, nomeTipoArquivo);
            parametros.Add(ConstantesCabecalho.NOME_ASSEMBLY_APLICACAO, nomeAssembly);
            parametros.Add(ConstantesCabecalho.IDENTIFICADOR_USUARIO, credencialUsuario.IdentificadorUsuario);
            parametros.Add(ConstantesCabecalho.SENHA, credencialUsuario.Senha);
            parametros.Add(ConstantesCabecalho.IDENTIFICADOR_SESSAO_USUARIO, identificadorSessao.ToString());
            parametros.Add(ConstantesCabecalho.IDENTIFICADOR_PROPRIETARIO, identificadorProprietario);

            var urlBaixaArquivo = ServicoArquivoClienteUtil.RetornarEnderecoBaixarArquivo(urlServico);
            var requisicao = (HttpWebRequest)WebRequest.Create(urlBaixaArquivo);

            foreach (var item in parametros)
            {
                requisicao.Headers.Add(item.Key, Base64Util.Encode(item.Value));
            }

            requisicao.Headers.Add(ConstantesCabecalho.TOKEN, HttpUtility.UrlEncode(token));

            requisicao.Timeout = (int)timeout.TotalMilliseconds;
            requisicao.Proxy = null;
            requisicao.ContentType = "application/octet-stream";
            requisicao.Method = "POST";

            var bytes = Guid.NewGuid().ToByteArray();
            requisicao.ContentLength = bytes.Length;
            using (var streamRequisicao = requisicao.GetRequestStream())
            {
                streamRequisicao.Write(bytes, 0, bytes.Length);
            }

            using (var resposta = (HttpWebResponse)requisicao.GetResponse())
            {
                using (var streamResposta = resposta.GetResponseStream())
                {
                    return StreamUtil.RetornarMemoryStreamBuferizada(streamResposta);
                }
            }
        }
    }
}
