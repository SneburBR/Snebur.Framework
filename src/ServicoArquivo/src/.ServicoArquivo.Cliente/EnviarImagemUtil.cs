using Snebur.Dominio;
using Snebur.Seguranca;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Snebur.ServicoArquivo.Cliente
{
    public class EnviarImagemUtil
    {
        public static ResultadoServicoArquivo EnviarImagem(string urlServicoImagem,
                                                           IImagem imagem,
                                                           EnumTamanhoImagem tamanhoImagem,
                                                           Stream stream)
        {
            var aplicacao = AplicacaoSnebur.Atual;

            return EnviarImagem(urlServicoImagem, imagem, stream, tamanhoImagem,
                               aplicacao.CredencialUsuario,
                               aplicacao.IdentificadorSessaoUsuario,
                               aplicacao.IdentificadorProprietario);
        }

        public static Task<ResultadoServicoArquivo> EnviarImagemAsync(IImagem imagem,
                                                                     EnumTamanhoImagem tamanhoImagem,
                                                                     Stream stream)
        {
            return Task.Factory.StartNew(() => EnviarImagem(imagem, tamanhoImagem, stream));
        }
        public static ResultadoServicoArquivo EnviarImagem(IImagem imagem,
                                                           EnumTamanhoImagem tamanhoImagem,
                                                           Stream stream)
        {
            var aplicacao = AplicacaoSnebur.Atual;
            var informacaoSessaoUsuario = aplicacao.InformacaoSessao;
            var credencialUsuario = aplicacao.CredencialUsuario;
            var identificadorSessaoUsuario = aplicacao.IdentificadorSessaoUsuario;
            
            return EnviarImagem(aplicacao.UrlServicoImagem, imagem, stream, tamanhoImagem,
                                credencialUsuario,
                                identificadorSessaoUsuario,
                                aplicacao.IdentificadorProprietario);
        }

        public static ResultadoServicoArquivo EnviarImagem(string urlServico, IImagem imagem,
                                                          EnumTamanhoImagem tamanhoImagem,
                                                          string caminhoArquivo,
                                                          Guid identificadorSessaoUsuario,
                                                          CredencialUsuario credencialUsuario,
                                                          string identificadorProprietario)
        {
            using (var fs = StreamUtil.OpenRead(caminhoArquivo))
            {
                return EnviarImagemUtil.EnviarImagem(urlServico, imagem, fs, tamanhoImagem, credencialUsuario, identificadorSessaoUsuario, identificadorProprietario);
            }
        }

        //public static ResultadoServicoArquivo EnviarImagem(string urlServico, IImagem imagem, Stream stream, EnumTamanhoImagem tamanhoImagem,
        //    Guid identificadorSessaoUsuario, CredencialUsuario credencialUsuario, string identificadorProprietario)
        //{
        //    return EnviarImagem(urlServico, imagem, stream, tamanhoImagem, identificadorSessaoUsuario, credencialUsuario, identificadorProprietario, 0);
        //}

        private static ResultadoServicoArquivo EnviarImagem(string urlServico,
                                                            IImagem imagem, Stream stream,
                                                            EnumTamanhoImagem tamanhoImagem,
                                                            CredencialUsuario credencialUsuario,
                                                            Guid identificadorSessaoUsuario,
                                                            string identificadorProprietario)
        {


            using (var enviadorImagemn = new EnviadorImagem(urlServico, imagem, tamanhoImagem, stream,
                                                            credencialUsuario, identificadorSessaoUsuario, identificadorProprietario))
            {
                enviadorImagemn.Enviar();
                return enviadorImagemn.Resultado;
            }
        }

        //private static ResultadoServicoArquivo EnviarImagem_Depreciado(string urlServico,
        //                                                               IImagem imagem,
        //                                                               Stream stream,
        //                                                               EnumTamanhoImagem tamanhoImagem,
        //                                                               Guid identificadorSessaoUsuario,
        //                                                               CredencialUsuario credencialUsuario,
        //                                                               string identificadorProprietario,
        //                                                               int tentativa)
        //{

        //    var totalBytes = stream.Length;
        //    var checksum = ChecksumUtil.RetornarChecksum(stream);
        //    var parametros = EnviarImagemUtil.RetornarParametrosCabacalho(imagem, tamanhoImagem, checksum, totalBytes, identificadorSessaoUsuario, credencialUsuario, identificadorProprietario);

        //    var urlEnviarImagem = ServicoImagemClienteUtil.RetornarEnderecoEnviarImagem(urlServico);
        //    var requisicao = (HttpWebRequest)WebRequest.Create(urlEnviarImagem);
        //    requisicao.Headers.Add(ConstantesCabecalho.IDENTIFICADOR_PROPRIETARIO, identificadorProprietario.ToString());

        //    foreach (var item in parametros)
        //    {
        //        requisicao.Headers.Add(item.Key, Base64Util.Encode(item.Value));
        //    }

        //    requisicao.Timeout = Int32.MaxValue;
        //    requisicao.Proxy = null;

        //    requisicao.ContentType = "application/octet-stream";
        //    requisicao.ContentLength = totalBytes;
        //    requisicao.Method = "POST";

        //    using (var streamRequisicao = requisicao.GetRequestStream())
        //    {
        //        StreamUtil.SalvarStreamBufferizada(stream, streamRequisicao);
        //        //streamRequisicao.Write(bytes, 0, bytes.Length);
        //    }

        //    using (var resposta = (HttpWebResponse)requisicao.GetResponse())
        //    {
        //        using (var streamResposta = resposta.GetResponseStream())
        //        {
        //            using (var streamReader = new StreamReader(streamResposta))
        //            {
        //                var json = streamReader.ReadLine();
        //                var resultado = JsonUtil.Deserializar<ResultadoServicoArquivo>(json, true);
        //                if (resultado.IsSucesso)
        //                {
        //                    return resultado;
        //                }

        //                switch (resultado.TipoErroServicoArquivo)
        //                {
        //                    case EnumTipoErroServicoArquivo.ChecksumArquivoDiferente:
        //                    case EnumTipoErroServicoArquivo.TotalBytesDiferente:
        //                    case EnumTipoErroServicoArquivo.ArquivoTempEmUso:

        //                        return EnviarImagem_Depreciado(urlServico, imagem, stream, tamanhoImagem, identificadorSessaoUsuario, credencialUsuario, identificadorProprietario, tentativa++);

        //                    case EnumTipoErroServicoArquivo.IdArquivoNaoExiste:

        //                        throw new Erro("O id arquivo da imagem não foi encotrado, caso a imagem estaja salva, certifique-se de comitar transacao antes de enviar a imagem");

        //                    case EnumTipoErroServicoArquivo.Desconhecido:

        //                        throw new Erro("Erro desconhecido ao enviar imagem");

        //                    default:

        //                        throw new Erro($"O {nameof(resultado.TipoErroServicoArquivo)} não suportado");
        //                }
        //            }
        //        }
        //    }
        //}

        //private static Dictionary<string, string> RetornarParametrosCabacalho(IImagem imagem, EnumTamanhoImagem tamanhoImagem, string checksum, long totalBytes, Guid identificadorSessaoUsuario, CredencialUsuario credencialUsuario, string identificadorProprietario)
        //{
        //    var tamanhoPacote = totalBytes;
        //    var parametros = new Dictionary<string, string>();
        //    parametros.Add(ConstantesServicoArquivo.ID_ARQUIVO, imagem.Id.ToString());
        //    parametros.Add(ConstantesServicoArquivo.TAMANHO_PACOTE, tamanhoPacote.ToString());
        //    parametros.Add(ConstantesServicoArquivo.CHECKSUM_ARQUIVO, checksum);
        //    parametros.Add(ConstantesServicoArquivo.CHECKSUM_PACOTE, checksum);
        //    parametros.Add(ConstantesServicoArquivo.TOTAL_PARTES, "1");
        //    parametros.Add(ConstantesServicoArquivo.PARTE_ATUAL, "1");
        //    parametros.Add(ConstantesServicoArquivo.TOTAL_BYTES, totalBytes.ToString());
        //    parametros.Add(ConstantesServicoArquivo.ASEMMBLY_QUALIFIED_NAME, imagem.GetType().AssemblyQualifiedName);

        //    parametros.Add(ConstantesCabecalho.IDENTIFICADOR_PROPRIETARIO, identificadorProprietario);
        //    parametros.Add(ConstantesCabecalho.IDENTIFICADOR_SESSAO_USUARIO, identificadorSessaoUsuario.ToString());
        //    parametros.Add(ConstantesCabecalho.IDENTIFICADOR_USUARIO, credencialUsuario.IdentificadorUsuario);
        //    parametros.Add(ConstantesCabecalho.SENHA, credencialUsuario.Senha);
        //    parametros.Add(ConstantesServicoArquivo.NOME_TIPO_ARQUIVO, imagem.GetType().Name);

        //    parametros.Add(ConstantesServicoImagem.TAMANHO_IMAGEM, ((int)tamanhoImagem).ToString());
        //    parametros.Add(ConstantesServicoImagem.FORMATO_IMAGEM, ((int)imagem.FormatoImagem).ToString());


        //    return parametros;
        //}


    }
}
