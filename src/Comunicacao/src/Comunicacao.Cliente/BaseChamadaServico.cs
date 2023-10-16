using Snebur.Seguranca;
using Snebur.Utilidade;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace Snebur.Comunicacao
{
    public abstract class BaseChamadaServico
    {
        private const int MAXIMO_TENTATIVA = 3;
        private string NomeManipulador { get; }
        public ContratoChamada ContratoChamada { get; }
        public string UrlServico { get; }
        public Type TipoRetorno { get; }
        public Dictionary<string, string> ParametrosCabecalhoAdicionais { get; }

        public BaseChamadaServico(string nomeManipualador,
            ContratoChamada constratoChamada,
            string urlServico,
            Type tipoRetorno,
            Dictionary<string, string> parametrosCabecalhoAdicionais)
        {
            this.NomeManipulador = nomeManipualador;
            this.ContratoChamada = constratoChamada;
            this.UrlServico = urlServico;
            this.TipoRetorno = tipoRetorno;
            this.ParametrosCabecalhoAdicionais = parametrosCabecalhoAdicionais;
        }

        protected object RetornarValorChamada()
        {
            var resultadoChamada = this.RetornarResultadoChamada();
            switch (resultadoChamada)
            {
                case ResultadoChamadaTipoPrimario resultadoTipoPrimario:

                    return ConverterUtil.ConverterTipoPrimario(resultadoTipoPrimario.Valor, resultadoTipoPrimario.TipoPrimarioEnum);

                case ResultadoChamadaEnum resultadoChamadaEnum:

                    return resultadoChamadaEnum.Valor;

                case ResultadoChamadaBaseDominio resultadoChamadaBaseDominio:

                    return resultadoChamadaBaseDominio.BaseDominio;

                case ResultadoChamadaLista resultadoChamadaLista:

                    return (object)this.RetornarResultadoChamadaLista(resultadoChamadaLista);

                case ResultadoChamadaVazio resultadoChamadaVazio:

                    return null;

                case ResultadoSessaoUsuarioInvalida resultadoSessaoUsuarioInvalida:

                    return resultadoSessaoUsuarioInvalida;

                case ResultadoChamadaErro resultadoChamadaErro:

                    throw new ErroNaoSuportado($"O  erro no resultado da chamada  {resultadoChamada?.GetType().Name ?? "null"} \r\n " +
                                               $" {resultadoChamadaErro.MensagemErro} ");

                default:

                    throw new ErroNaoSuportado($"O resultado da chamada não é suportado {resultadoChamada?.GetType().Name ?? "null"}");
            }
        }

        private IList RetornarResultadoChamadaLista(ResultadoChamadaLista resultadoLista)
        {
            var tipoItem = Type.GetType(resultadoLista.AssemblyQualifiedName);
            var tipoLista = typeof(List<>).MakeGenericType(tipoItem);
            var listaTipada = (IList)Activator.CreateInstance(tipoLista);
            var lista = this.RetornarListaResultadoChamadaLista(resultadoLista);
            foreach (var item in lista)
            {
                listaTipada.Add(item);
            }
            return listaTipada;
        }

        private ICollection RetornarListaResultadoChamadaLista(ResultadoChamadaLista resultadoLista)
        {
            if (resultadoLista is ResultadoChamadaListaBaseDominio listaBasedominio)
            {
                return listaBasedominio.BasesDominio;
            }

            if (resultadoLista is ResultadoChamadaListaTipoPrimario listaTipoPimario)
            {
                return listaTipoPimario.Valores;
            }

            if (resultadoLista is ResultadoChamadaListaEnum resultadoListaEnum)
            {
                return resultadoListaEnum.Valores;
            }
            throw new ErroNaoSuportado(String.Format("O resultado da chamada não é suportado"));
        }

        private ResultadoChamada RetornarResultadoChamada(int tentativa = 0)
        {
            var nomeAssembly = AplicacaoSnebur.Atual.NomeAplicacao;
            var conteudo = this.RetornarConteudoCompactado();
            var token = Token.RetornarNovoToken();
            var nomeArquivo = Md5Util.RetornarHash(token);

            var urlServico = UriUtil.CombinarCaminhos(this.UrlServico, nomeArquivo);

            var requisicaoHttp = HttpWebRequest.Create(urlServico);

            var identificadorUsuario = CriptografiaUtil.Criptografar(token, this.ContratoChamada.Cabecalho.CredencialUsuario.IdentificadorUsuario);
            var identifcadorProprietario = this.ContratoChamada.Cabecalho.IdentificadorProprietario;
        
            requisicaoHttp.Headers[ConstantesCabecalho.IDENTIFICADOR_USUARIO] = identificadorUsuario;
            requisicaoHttp.Headers[ConstantesCabecalho.SENHA] = CriptografiaUtil.Criptografar(token, this.ContratoChamada.Cabecalho.CredencialServico.Senha);
            requisicaoHttp.Headers[ConstantesCabecalho.NOME_ASSEMBLY_APLICACAO] = nomeAssembly;

            if (!String.IsNullOrEmpty(identifcadorProprietario))
            {
                requisicaoHttp.Headers[ConstantesCabecalho.IDENTIFICADOR_PROPRIETARIO] = identifcadorProprietario;
            }
            else
            {
                var aplicacao = AplicacaoSnebur.Atual;
                if (aplicacao.IsAplicacaoAspNet && aplicacao.AspNet.IsPossuiRequisicaoAspNetAtiva)
                {
                    //var identificadorProprietario = AplicacaoSneburAspNet.AtualAspNet?.HttpContext?.Request.Headers[ConstantesCabecalho.IDENTIFICADOR_PROPRIETARIO];
                    var identificadorProprietario = aplicacao.AspNet.RetornarValueCabecalho(ConstantesCabecalho.IDENTIFICADOR_PROPRIETARIO);
                    if (!String.IsNullOrEmpty(identificadorProprietario))
                    {
                        requisicaoHttp.Headers[ConstantesCabecalho.IDENTIFICADOR_PROPRIETARIO] = identificadorProprietario;
                    }
                }
            }

            if (this.ParametrosCabecalhoAdicionais != null)
            {
                foreach (var parametroAdicional in this.ParametrosCabecalhoAdicionais)
                {
                    requisicaoHttp.Headers[parametroAdicional.Key] = parametroAdicional.Value;
                }
            }
            requisicaoHttp.ContentType = "application/octet-stream";
            requisicaoHttp.ContentLength = conteudo.Length;
            requisicaoHttp.Method = "POST";
             
            requisicaoHttp.Headers.Add(ParametrosComunicacao.TOKEN, WebUtility.UrlEncode(token));
            requisicaoHttp.Headers.Add(ParametrosComunicacao.MANIPULADOR, this.NomeManipulador);
            requisicaoHttp.Timeout = (int)TimeSpan.FromMinutes(2).TotalMilliseconds;

#if DEBUG
            if (DebugUtil.IsAttached)
            {
                requisicaoHttp.Timeout = (int)TimeSpan.FromHours(1).TotalMilliseconds;
            }
#endif
            try
            {
                using (var streamRequisicao = requisicaoHttp.GetRequestStream())
                {
                    streamRequisicao.Write(conteudo, 0, conteudo.Length);
                }

                using (var resposta = (HttpWebResponse)requisicaoHttp.GetResponse())
                {
                    if (!((resposta is HttpWebResponse) && (resposta as HttpWebResponse).StatusCode == HttpStatusCode.OK))
                    {
                        throw new ErroComunicacao("Falha de comunicação com servidor");
                    }
                    using (var streamResposta = resposta.GetResponseStream())
                    {
                        InternetUtil.FecharMensagemSemInternet();
                        var jsonResposta = PacoteUtil.DescompactarPacote(streamResposta);
                        return JsonUtil.Deserializar<ResultadoChamada>(jsonResposta, EnumTipoSerializacao.DotNet);
                    }
                }
            }
            catch (Exception)
            {
                if (!RedeUtil.InternetConectada())
                {
                    //tentativa = 0;
                    InternetUtil.AguardarRestabelecerInternet();
                }

                if (AplicacaoSnebur.Atual.TipoAplicacao == Dominio.EnumTipoAplicacao.DotNet_WebService ||
                    tentativa > MAXIMO_TENTATIVA)
                {
                    throw;
                }
                return this.RetornarResultadoChamada(tentativa + 1);
            }
        }

        private byte[] RetornarConteudoCompactado()
        {
            var jsonString = JsonUtil.Serializar(this.ContratoChamada, EnumTipoSerializacao.DotNet);
            return PacoteUtil.CompactarPacote(jsonString);
        }

        private static void DefinirExpect100Continue()
        {
            ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.DefaultConnectionLimit = 1000;
        }
    }
}