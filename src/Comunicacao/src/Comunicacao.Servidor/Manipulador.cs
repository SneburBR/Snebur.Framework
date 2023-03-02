using Snebur.Seguranca;
using Snebur.Servicos;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;

namespace Snebur.Comunicacao
{
    /// <summary>
    /// Controla todas as requisições das url dinâmicas do servidos do WebService
    /// Valida o token,e chama o manipulador do Servido informado na cabeçalho do requisição do lado cliente
    /// </summary>
    public abstract class BaseManipuladorRequisicao : IHttpModule
    {
        private const int TEMPO_EXPIRAR_TOKEN = Token.TEMPO_EXPIRAR_TOKEN_PADRAO;

        private Dictionary<string, Type> _tiposManipuladore;

        private Dictionary<string, Type> Manipuladores { get; } = new Dictionary<string, Type>();
        private Dictionary<string, (Type tipo, bool isValidarToken)> ManipuladoresGenericos { get; } = new Dictionary<string, (Type, bool)>();
        private Dictionary<string, bool> ArquivosAutorizados { get; } = new Dictionary<string, bool>();

        private Dictionary<string, Type> TiposManipuladores => LazyUtil.RetornarValorLazyComBloqueio(ref _tiposManipuladore, this.RetornarTiposManipuladores);
        protected bool IsAutoRegistrarManipulador { get; set; } = true;
        private readonly object _bloqueio = new object();

        public BaseManipuladorRequisicao()
        {
            //this.AutorizarArquivo("clientaccesspolicy.xml");
            //this.AutorizarArquivo("crossdomain.xml", true);
            this.AutorizarArquivo("favicon.ico", true);
            this.InicializarManipuladores();
        }

        public abstract void InicializarManipuladores();

        public void Init(HttpApplication aplicacao)
        {
            aplicacao.BeginRequest += (new EventHandler(this.Aplicacao_BeginRequest));
        }

        private void Aplicacao_BeginRequest(object sender, EventArgs e)
        {
            var aplicacao = (HttpApplication)sender;
            this.AntesProcessarRequisicao(aplicacao.Context);

            var request = aplicacao.Request;
            var response = aplicacao.Context.Response;

            if (this.IsExecutarServico(aplicacao))
            {
                response.StatusCode = 0;
                try
                {
                    var allKeys = request.Headers.AllKeys;
                    if (allKeys.Contains(ParametrosComunicacao.TOKEN, new IgnorarCasoSensivel()) &&
                        allKeys.Contains(ParametrosComunicacao.MANIPULADOR, new IgnorarCasoSensivel()))
                    {
                        this.ExecutarServico(aplicacao);
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
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    }

                    if (DebugUtil.IsAttached)
                    {
                        throw ex;
                    }
                    LogUtil.ErroAsync(ex);
                }
                finally
                {
                    if (response.StatusCode == 0)
                    {
                        response.StatusCode = (int)HttpStatusCode.Conflict;
                        response.SubStatusCode = -1;
                    }
                    aplicacao.CompleteRequest();
                }
            }
        }

        private bool IsExecutarServico(HttpApplication aplicacao)
        {
            var request = aplicacao.Request;
            var response = aplicacao.Context.Response;
            var caminho = Path.GetFileName(request.Url.LocalPath).ToLower();

            if (HttpUtil.VerificarContratoCrossDomain(aplicacao.Context))
            {
                aplicacao.CompleteRequest();
                return false;
            }

            if (caminho.Equals("agora", StringComparison.InvariantCultureIgnoreCase))
            {
                this.ResponderAgora(response);
                aplicacao.CompleteRequest();
                return false;
            }

            if (caminho.Equals("ping", StringComparison.InvariantCultureIgnoreCase))
            {
                response.Write("True");
                aplicacao.CompleteRequest();
                return false;
            }

            if (this.ArquivosAutorizados.ContainsKey(caminho))
            {
                var isIgnorarValidacaoTokenAplicacao = this.ArquivosAutorizados[caminho];
                if (!isIgnorarValidacaoTokenAplicacao)
                {
                    if (!this.IsRequicaoValida(request, false))
                    {
                        response.StatusCode = (int)HttpStatusCode.Unauthorized ;
                        aplicacao.CompleteRequest();
                        return false;
                    }
                }
                return false;
            }

            var caminhoManipulador = Path.GetFileNameWithoutExtension(caminho);
            if (this.ManipuladoresGenericos.ContainsKey(caminhoManipulador))
            {
                var isValidarToken = this.ManipuladoresGenericos[caminhoManipulador].isValidarToken;
                if (isValidarToken && !this.IsRequicaoValida(request, false))
                {

                    response.StatusCode =  (int)HttpStatusCode.Unauthorized; 

                    aplicacao.CompleteRequest();
                    return false;
                }

                this.ExecutarManipuladorGenerico(aplicacao.Context, caminhoManipulador);
                aplicacao.CompleteRequest();
                return false;
            }

            
            return true;

        }

        private void ExecutarManipuladorGenerico(HttpContext httpContext, string caminho)
        {
            var tipo = this.ManipuladoresGenericos[caminho].tipo;
            var manipualador = Activator.CreateInstance(tipo) as IHttpHandler;
            if (manipualador != null)
            {
                manipualador.ProcessRequest(httpContext);
            }
        }

        #region Métodos privados

        private void ExecutarServico(HttpApplication aplicacao)
        {
            var request = aplicacao.Request;

            if (this.IsRequicaoValida(request, true))
            {
                var identificadorProprietario = this.RetornarIdentificadorProprietario(request);
                var nomeManipulador = request.Headers[ParametrosComunicacao.MANIPULADOR];
                var tipoManipulador = this.RetornarTipoServico(nomeManipulador);
                using (var servico = (BaseComunicacaoServidor)Activator.CreateInstance(tipoManipulador))
                {
                    try
                    {
                        servico.IdentificadorProprietario = identificadorProprietario;
                        servico.ProcessRequest(aplicacao.Context);
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

            var identificadorProprietario = this.RetornarIdentificadorProprietario(request);
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
                           $"Cabeçalho {ConstantesCabecalho.IDENTIFICADOR_PROPRIETARIO}: '{identificadorPropriedadetarioNoCabecalho}'\r\n" +
                           $"Cabeçalho {ConstantesCabecalho.ORIGIN}: '{origem}'\r\n" +
                           $"Cabeçalho {ConstantesCabecalho.NOME_ASSEMBLY_APLICACAO}: '{nomeAssembly}'\r\n" +
                           $"Cabeçalho {ParametrosComunicacao.NOME_APLICACAO_WEB}: '{nomeAplicacaoWeb}'\r\n" +
                           $"Ambiente IIS : {ambienteIIS}\r\n";


            LogUtil.SegurancaAsync(mensagem, tipoLogSeguranca);
        }

        private bool ValidarToken(HttpRequest request, string token, bool isValidarUrlMd5)
        {
            var resultado = Token.ValidarToken(token, TimeSpan.FromSeconds(TEMPO_EXPIRAR_TOKEN));
            if (resultado.Estado != EnumEstadoToken.Valido)
            {
                var tipoLogSeguranca = resultado.RetornarTipoLogReguranca();
                var mensagem = String.Format("Token : {0} - DataHora requisição : {1}, DataHora Token {2}", token, DateTime.UtcNow, resultado.DataHora);
                LogUtil.SegurancaAsync(mensagem, tipoLogSeguranca);
                return false;
            }

            if (isValidarUrlMd5)
            {
                var arquivo = Path.GetFileNameWithoutExtension(UriUtil.RemoverBarraInicial(request.Url.LocalPath).ToLower());
                var tokenMd5 = Md5Util.RetornarHash(token);
                if (arquivo != tokenMd5)
                {
                    var mensagem = String.Format("Token: {0} - Url requisição inválida {1}", token, request.UrlReferrer?.AbsoluteUri);
                    LogUtil.SegurancaAsync(mensagem, Servicos.EnumTipoLogSeguranca.UrlNaoAutorizada);
                    return false;
                }
            }
            return true;
        }

        private Type RetornarTipoServico(string nomeServico)
        {
            if (!this.Manipuladores.ContainsKey(nomeServico))
            {
                this.TentarRegistrarManipulador(nomeServico);
                if (!this.Manipuladores.ContainsKey(nomeServico))
                {
                    throw new ErroManipualdorNaoEncontrado(String.Format("O manipulador {0} não foi encontrado, deve ser inicializado no construtor do manipulador do WebService", nomeServico));
                }
            }
            return this.Manipuladores[nomeServico];
        }

        private void TentarRegistrarManipulador(string nomeServico)
        {
            if (this.IsAutoRegistrarManipulador)
            {
                lock (this._bloqueio)
                {
                    if (!this.Manipuladores.ContainsKey(nomeServico))
                    {
                        var tipo = this.TiposManipuladores.GetValueOrDefault(nomeServico);
                        if (tipo != null)
                        {
                            this.Manipuladores.Add(nomeServico, tipo);
                        }
                    }
                }
            }
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

        protected void AdicionarServico<T>(string nomeServico) where T : BaseComunicacaoServidor
        {
            var tipo = typeof(T);
            this.AdicionarServico(nomeServico, tipo);
        }

        protected void AdicionarServico<T>() where T : BaseComunicacaoServidor
        {
            var tipo = typeof(T);
            this.AdicionarServico(tipo.Name, tipo);
        }
        protected void AdicionarServico(string nome, Type tipo)
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

        protected void AdicionarManipuladorGenerico<T>(bool isValidarToken) where T : IHttpHandler
        {
            var tipo = typeof(T);
            this.AdicionarManipuladorGenerico<T>(tipo.Name.ToLower(), isValidarToken);
        }

        protected void AdicionarManipuladorGenerico<T>(string nomeManipulador, bool isValidarToken) where T : IHttpHandler
        {
            var tipo = typeof(T);
            var chave = nomeManipulador.ToLower();
            if (this.ManipuladoresGenericos.ContainsKey(chave))
            {
                throw new Exception($"Já foi adicionado um manipulador para {chave}");
            }
            this.ManipuladoresGenericos.Add(chave, (tipo, isValidarToken));
        }

        #endregion

        #region Métodos virtuais


        protected virtual void AntesProcessarRequisicao(HttpContext context)
        {

        }


        #endregion

        #region Métodos abstratos

        protected abstract string RetornarIdentificadorProprietario(HttpRequest httpRequest);

        protected abstract bool IsAplicacaoAutorizada(HttpRequest request);

        #endregion

        #region Métodos privados
        private Dictionary<string, Type> RetornarTiposManipuladores()
        {
            var retorno = new Dictionary<string, Type>();
            var tiposServicos = new List<Type>();
            var tipoServico = typeof(BaseComunicacaoServidor);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var tipo in assembly.GetLoadableTypes())
                {
                    if (tipo.IsSubclassOf(tipoServico))
                    {
                        tiposServicos.Add(tipo);
                    }
                }
            }
            var tipos = tiposServicos.GroupBy(x => x.Name).Where(x => x.Count() == 1).Select(x => x.Single());
            return tipos.ToDictionary(x => x.Name);
        }
        #endregion

        public void Dispose()
        {
        }

    }
}
