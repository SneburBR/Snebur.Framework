using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Snebur.Seguranca;
using Snebur.Servicos;
using Snebur.Utilidade;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Snebur.Comunicacao
{

    /// <summary>
    /// Controla todas as requisicoes das url dinamicas do servidos do WebService
    /// Valida o token,e chama o manipulador do Servido informado na cabeçalho do requisicao do lado cliente
    /// </summary>
    public abstract class BaseManipuladorRequisicao  
    {

        private const int TEMPO_EXPIRAR_TOKEN = Token.TEMPO_EXPIRAR_TOKEN_PADRAO;
        private Dictionary<string, Type> Manipuladores { get; } = new Dictionary<string, Type>();
        private Dictionary<string, bool> ArquivosAutorizados { get; } = new Dictionary<string, bool>();

        private readonly RequestDelegate _next;

        public BaseManipuladorRequisicao()
        {
            //this.AutorizarArquivo("clientaccesspolicy.xml");
            //this.AutorizarArquivo("crossdomain.xml", true);
            this.AutorizarArquivo("favicon.ico", true);
            this.InicializarManipuladores();
        }

        public MyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Do something with context near the beginning of request processing.

            await _next.Invoke(context);

            // Clean up.
        }


        public abstract void InicializarManipuladores();

        public void Init(HttpApplication aplicacao)
        {
            aplicacao.BeginRequest += (new EventHandler(this.Aplicacao_BeginRequest));
        }

        private void Aplicacao_BeginRequest(object sender, EventArgs e)
        {
            var aplicacao = (HttpApplication)sender;
            this.AntesRequisicao(aplicacao);
            var request = aplicacao.Request;
            var response = aplicacao.Context.Response;

            if (this.ContinuarRequisicao(aplicacao))
            {
                response.StatusCode = 0;
                try
                {
                    if (request.Headers.AllKeys.Contains(ParametrosComunicacao.TOKEN) &&
                        request.Headers.AllKeys.Contains(ParametrosComunicacao.MANIPULADOR))
                    {
                        this.ChamarManipulador(aplicacao);
                    }
                    else
                    {
                        LogUtil.SegurancaAsync(String.Format("A url '{0}' foi chamada incorretamente.", request.Url.AbsoluteUri), Servicos.EnumTipoLogSeguranca.CabecalhoInvalido);
                    }
                    //Chamadas do cross domain do ajax serão implementas aqui
                }
                catch (Exception ex)
                {
                    var host = aplicacao.Request.Url.Host;
                    if (host.EndsWith(".local") || host.EndsWith("interno"))
                    {
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    }
                    else
                    {
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                    }

                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        throw ex;
                    }
                    LogUtil.ErroAsync(ex);
                }
                finally
                {
                    if (response.StatusCode == 0)
                    {
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        response.SubStatusCode = -1;
                    }
                    aplicacao.CompleteRequest();
                }
            }
        }

        private bool ContinuarRequisicao(HttpContext httpContext)
        {
            var request = httpContext.Request;
            var response = httpContext.Response;
            var arquivo = Path.GetFileName(request.Url.LocalPath).ToLower();

            if (HttpUtil.VerificarContratoCrossDomain(httpContext))
            {
                httpContext.CompleteRequest();
                return false;
            }

            if (arquivo.Equals("agora", StringComparison.InvariantCultureIgnoreCase))
            {
                this.ResponderAgora(response);
                httpContext.CompleteRequest();
                return false;
            }

            if (arquivo.Equals("ping", StringComparison.InvariantCultureIgnoreCase))
            {
                response.Write("True");
                httpContext.CompleteRequest();
                return false;
            }

            if (this.ArquivosAutorizados.ContainsKey(arquivo))
            {
                var isIgnorarValidacaoTokenAplicacao = this.ArquivosAutorizados[arquivo];
                if (!isIgnorarValidacaoTokenAplicacao)
                {
                    if (!this.IsRequicaoValida(request, false))
                    {
                        response.StatusCode = 404;
                        httpContext.CompleteRequest();
                        return false;
                    }
                }
                return false;

            }
            return true;

        }

        #region Métodos privados

        private void ChamarManipulador(HttpContext httpContext)
        {
            var request = httpContext.Request;

            if (this.IsRequicaoValida(request, true))
            {
                var identificadorProprietario = this.RetornarIdentificadorProprietario();
                var nomeManipulador = request.Headers[ParametrosComunicacao.MANIPULADOR];
                var tipoManipulador = this.RetornarTipoManipulador(nomeManipulador);
                using (var manipulador = (BaseComunicacaoServidor)Activator.CreateInstance(tipoManipulador))
                {
                    try
                    {
                        manipulador.IdentificadorProprietario = identificadorProprietario;
                        manipulador.ProcessRequest(httpContext);
                    }
                    catch (ErroRequisicao ex)
                    {
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        throw new ErroWebService(ex, nomeManipulador);
                    }
                }
            }
        }

        private bool IsRequicaoValida(HttpRequest request, bool isValidarUrlMd5)
        {
            if (!this.IsAplicacaoAutorizada(request))
            {
                this.NotificarLogSeguranca(request, EnumTipoLogSeguranca.AplicacaoNaoAutorizada);
                return false;
            }

            var identificadorProprietario = this.RetornarIdentificadorProprietario();
            if (String.IsNullOrWhiteSpace(identificadorProprietario))
            {
                this.NotificarLogSeguranca(request, EnumTipoLogSeguranca.IdentificadorProprietarioInvalido);
                return false;
            }
            var token = HttpUtility.UrlDecode(request.Headers[ParametrosComunicacao.TOKEN]);
            return this.ValidarToken(request, token, isValidarUrlMd5);
        }

        private void NotificarLogSeguranca(HttpRequest request,
                                          EnumTipoLogSeguranca tipoLogSeguranca)
        {
            var host = request.UrlReferrer?.Host?.ToLower() ?? " host não definida";
            var url = request?.UrlReferrer?.AbsoluteUri ?? " url não definida";

            var identificadorPropriedadetarioNoCabecalho = request.Headers[ConstantesCabecalho.IDENTIFICADOR_PROPRIETARIO] ??
                                                          $"{ConstantesCabecalho.IDENTIFICADOR_PROPRIETARIO} não definido no cabeçalho ";

            var origem = request.Headers[ConstantesCabecalho.ORIGIN] ?? "Origem não definida";
            var ambienteIIS = ConfiguracaoUtil.AmbienteServidor.ToString();
            var nomeAssembly = HttpContext.Current.Request?.Headers[ConstantesCabecalho.NOME_ASSEMBLY_APLICACAO];
            var nomeAplicacaoWeb = HttpContext.Current.Request?.Headers[ParametrosComunicacao.NOME_APLICACAO_WEB];

            var mensagem = $"Host: '{host}'\r\n" +
                           $"Url: '{url}' \r\n" +
                           $"Cabecalho {ConstantesCabecalho.IDENTIFICADOR_PROPRIETARIO}: '{identificadorPropriedadetarioNoCabecalho}'\r\n" +
                           $"Cabeçalho {ConstantesCabecalho.ORIGIN}: '{origem}'\r\n" +
                           $"Cabeçalho {ConstantesCabecalho.NOME_ASSEMBLY_APLICACAO}: '{nomeAssembly}'\r\n" +
                           $"Cabeçalho {ParametrosComunicacao.NOME_APLICACAO_WEB}: '{nomeAplicacaoWeb}'\r\n" +
                           $"Ambiente Iis : {ambienteIIS}\r\n";


            LogUtil.SegurancaAsync(mensagem, tipoLogSeguranca);
        }

        private bool ValidarToken(HttpRequest request, string token, bool isValidarUrlMd5)
        {
            var resultado = Token.ValidarToken(token, TimeSpan.FromSeconds(TEMPO_EXPIRAR_TOKEN));
            if (resultado.Estado != EnumEstadoToken.Valido)
            {
                var tipoLogSeguranca = resultado.RetornarTipoLogReguranca();
                var mensagem = String.Format("Token : {0} - DataHora requisicao : {1}, DataHora Token {2}", token, DateTime.UtcNow, resultado.DataHora);
                LogUtil.SegurancaAsync(mensagem, tipoLogSeguranca);
                return false;
            }

            if (isValidarUrlMd5)
            {
                var arquivo = Path.GetFileNameWithoutExtension(UriUtil.RemoverBarraInicial(request.Url.LocalPath).ToLower());
                var tokenMd5 = Md5Util.RetornarHash(token);
                if (arquivo != tokenMd5)
                {
                    var mensagem = String.Format("Token: {0} - Url requisao invalida {1}", token, request.UrlReferrer?.AbsoluteUri);
                    LogUtil.SegurancaAsync(mensagem, Servicos.EnumTipoLogSeguranca.UrlNaoAutorizada);
                    return false;
                }
            }

            return true;

        }

        private Type RetornarTipoManipulador(string nomeManipualdor)
        {
            if (!this.Manipuladores.ContainsKey(nomeManipualdor))
            {
                throw new ErroManipualdorNaoEncontrado(String.Format("O manipualdor {0} não foi encontrado, deve ser inicializado no construtor do manipulador do WebService", nomeManipualdor));
            }
            return this.Manipuladores[nomeManipualdor];
        }

        private void ResponderAgora(HttpResponse response)
        {
            var agora = DateTime.UtcNow.AddSeconds(-10);
            response.ContentEncoding = Encoding.UTF8;
            response.ContentType = "text/text";
            response.Charset = "utf8";
            response.StatusCode = 200;
            response.Write(agora.Ticks);
        }

        #endregion

        #region Métodos compartilhados

        protected void AutorizarArquivo(string nomeArquivo,
                                        bool isIgnorarValidacaoTokenAplicacao = false)
        {
            this.ArquivosAutorizados.Add(nomeArquivo.ToLower(), isIgnorarValidacaoTokenAplicacao);
        }

        protected void AdicionarManipulador(string nome, Type tipo)
        {
            if (this.Manipuladores.ContainsKey(nome))
            {
                throw new Exception(String.Format("Já foi adicionado um manipulador para {0}", nome));
            }

            if (!this.Manipuladores.ContainsKey(nome))
            {
                if (!tipo.IsSubclassOf(typeof(BaseComunicacaoServidor)))
                {
                    throw new Exception(String.Format("O tipo não é suportado, ele deve herda de {0}", typeof(BaseComunicacaoServidor).Name));
                }
                this.Manipuladores.Add(nome, tipo);
            }
        }


        #endregion

        #region Métodos virtuais

        public virtual void AntesRequisicao(HttpApplication aplicacao)
        {

        }


        #endregion

        #region Métodos abstratos

        protected abstract string RetornarIdentificadorProprietario();

        protected abstract bool IsAplicacaoAutorizada(HttpRequest request);

        #endregion


        public void Dispose()
        {
        }

    }
}
