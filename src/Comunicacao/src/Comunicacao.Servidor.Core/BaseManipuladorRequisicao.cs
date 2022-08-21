using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Snebur.Seguranca;
using Snebur.Servicos;
using Snebur.Utilidade;

#if NET50
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
#endif

namespace Snebur.Comunicacao
{

    /// <summary>
    /// Controla todas as requisicoes das url dinamicas do servidos do WebService
    /// Valida o token,e chama o manipulador do Servido informado na cabeçalho do requisicao do lado cliente
    /// </summary>
    public abstract class BaseManipuladorRequisicao : IHttpModule
    {
        private const int TEMPO_EXPIRAR_TOKEN = Token.TEMPO_EXPIRAR_TOKEN_PADRAO;
        private Dictionary<string, Type> Servicos { get; } = new Dictionary<string, Type>();

        private Dictionary<string, (Type tipo, bool isValidarToken)> ManipuladoresGenericos { get; } = new Dictionary<string, (Type, bool)>();

        private Dictionary<string, bool> ArquivosAutorizados { get; } = new Dictionary<string, bool>();

        public BaseManipuladorRequisicao()
        {
            //this.AutorizarArquivo("clientaccesspolicy.xml");
            //this.AutorizarArquivo("crossdomain.xml", true);
            this.AutorizarArquivo("favicon.ico", true);
            this.AutorizarArquivo("favicon.png", true);
            this.InicializarManipuladores();
        }

        public abstract void InicializarManipuladores();

        protected async Task ProcessarRequisicaoAsync(HttpContext httpContext)
        {
            var request = httpContext.Request;
            var response = httpContext.Response;

            if (await this.IsExecutarServico(httpContext))
            {
                //response.StatusCode = 0;
                try
                {
                    if (request.Headers.Keys.Contains(ParametrosComunicacao.TOKEN) &&
                        request.Headers.Keys.Contains(ParametrosComunicacao.MANIPULADOR))
                    {
                        await this.ExecutarServicoAsyc(httpContext);
                    }
                    else
                    {
                        var urlCompleta = httpContext.Request.GetTypedHeaders()?.Referer?.AbsoluteUri;
                        LogUtil.SegurancaAsync($"A url '{urlCompleta}' foi chamada incorretamente.", EnumTipoLogSeguranca.CabecalhoInvalido);
                    }
                    //Chamadas do cross domain do ajax serão implementas aqui
                }
                catch (Exception ex)
                {

                    if (Debugger.IsAttached)
                    {
                        throw;
                    }

                    var host = httpContext.Request.GetTypedHeaders()?.Referer?.Host;
                    if (!response.HasStarted)
                    {
                        var statusCode = (host.EndsWith(".local") || host.EndsWith("interno")) ?
                                                (int)HttpStatusCode.InternalServerError :
                                                (int)HttpStatusCode.NotFound;

                        response.StatusCode = statusCode;
                    }


                    LogUtil.ErroAsync(ex);
                }
                finally
                {
                    if (!response.HasStarted && response.StatusCode == 0)
                    {
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                    }
                    httpContext.CompleteRequest();
                }
            }
        }

        private async Task<bool> IsExecutarServico(HttpContext httpContext)
        {
            var request = httpContext.Request;
            var response = httpContext.Response;

            var caminho = Path.GetFileName(request.Path.Value)?.ToLower();

            if (CrossDomainUtil.VerificarContratoCrossDomain(httpContext))
            {
                httpContext.CompleteRequest();
                return false;
            }

            if (caminho.Equals("agora", StringComparison.InvariantCultureIgnoreCase))
            {
                await this.ResponderAgoraAsync(response);
                httpContext.CompleteRequest();
                return false;
            }

            if (caminho.Equals("ping", StringComparison.InvariantCultureIgnoreCase))
            {
                await response.WriteAsync("true", Encoding.UTF8);
                httpContext.CompleteRequest();
                return false;
            }

            var caminhoManipulador = Path.GetFileNameWithoutExtension(caminho);
            if (this.ManipuladoresGenericos.ContainsKey(caminhoManipulador))
            {
                var isValidarToken = this.ManipuladoresGenericos[caminhoManipulador].isValidarToken;
                if (isValidarToken && !this.IsRequicaoValida(request, false))
                {
                    if (Debugger.IsAttached)
                    {
                        throw new Exception("Falha na validação do Token");
                    }

                    if (!response.HasStarted)
                    {
                        response.StatusCode = 404;
                    }
                    httpContext.CompleteRequest();
                    return false;
                }

                await this.ExecutarManipuladorGenericoAsync(httpContext, caminhoManipulador);
                return false;
            }

            if (this.ArquivosAutorizados.ContainsKey(caminho))
            {
                var isIgnorarValidacaoTokenAplicacao = this.ArquivosAutorizados[caminho];
                if (!isIgnorarValidacaoTokenAplicacao)
                {
                    if (!this.IsRequicaoValida(request, false))
                    {
                        if (!response.HasStarted)
                        {
                            response.StatusCode = 404;
                        }

                        httpContext.CompleteRequest();
                        return false;
                    }
                }
                return false;

            }
            return true;

        }

        private async Task ExecutarManipuladorGenericoAsync(HttpContext httpContext, string caminho)
        {
            var tipo = this.ManipuladoresGenericos[caminho].tipo;
            var manipualador = Activator.CreateInstance(tipo) as IHttpHandler;
            if (manipualador != null)
            {
                await manipualador.ProcessRequestAsync(httpContext);
            }
        }

        #region Métodos privados

        private async Task ExecutarServicoAsyc(HttpContext httpContext)
        {
            var request = httpContext.Request;

            if (this.IsRequicaoValida(request, true))
            {
                var identificadorProprietario = this.RetornarIdentificadorProprietario(request);
                var nomeServico = request.Headers[ParametrosComunicacao.MANIPULADOR];
                var tipoServico = this.RetornarTipoServico(nomeServico);
                using (var servico = (BaseComunicacaoServidor)Activator.CreateInstance(tipoServico))
                {
                    try
                    {
                        servico.IdentificadorProprietario = identificadorProprietario;
                        await servico.ProcessRequestAsyc(httpContext);
                    }
                    catch (ErroRequisicao)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        throw new ErroWebService(ex, nomeServico);
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
            var uri = request.GetTypedHeaders()?.Referer;
            var host = uri?.Host ?? " host não definida";
            var url = uri?.AbsoluteUri ?? " url não definida";

            var identificadorPropriedadetarioNoCabecalho = request.Headers.GetValue(ConstantesCabecalho.IDENTIFICADOR_PROPRIETARIO) ??
                                                                 $"{ConstantesCabecalho.IDENTIFICADOR_PROPRIETARIO} não definido no cabeçalho ";

            var origem = request.Headers.GetValue(ConstantesCabecalho.ORIGIN) ?? "Origem não definida";
            var ambienteIIS = ConfiguracaoUtil.AmbienteServidor.ToString();
            var nomeAssembly = request.Headers[ConstantesCabecalho.NOME_ASSEMBLY_APLICACAO];
            var nomeAplicacaoWeb = request.Headers[ParametrosComunicacao.NOME_APLICACAO_WEB];

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
                var arquivo = Path.GetFileNameWithoutExtension(request.Path);
                var tokenMd5 = Md5Util.RetornarHash(token);
                if (arquivo != tokenMd5)
                {
                    var url = request.GetTypedHeaders()?.Referer?.AbsoluteUri;
                    var mensagem = $"Token: {token} - Url requisao invalida {url}";
                    LogUtil.SegurancaAsync(mensagem, Snebur.Servicos.EnumTipoLogSeguranca.UrlNaoAutorizada);
                    return false;
                }
            }

            return true;

        }

        private Type RetornarTipoServico(string nomeManipuladorServidor)
        {
            if (!this.Servicos.ContainsKey(nomeManipuladorServidor))
            {
                throw new ErroManipualdorNaoEncontrado(String.Format("O manipualdor do servico {0} não foi encontrado, deve ser inicializado no construtor do manipulador do WebService", nomeManipuladorServidor));
            }
            return this.Servicos[nomeManipuladorServidor];
        }

        private async Task ResponderAgoraAsync(HttpResponse response)
        {
            var agora = DateTime.UtcNow.AddSeconds(-10);
            response.ContentType = "text/text; charset=UTF-8";
            if (!response.HasStarted)
            {
                response.StatusCode = 200;
            }
            await response.WriteAsync(agora.Ticks.ToString(), Encoding.UTF8);

        }

        #endregion

        #region Métodos compartilhados

        protected void AutorizarArquivo(string nomeArquivo,
                                        bool isIgnorarValidacaoTokenAplicacao = false)
        {
            this.ArquivosAutorizados.Add(nomeArquivo.ToLower(), isIgnorarValidacaoTokenAplicacao);
        }

        protected void AdicionarServico(string nome, Type tipo)
        {
            if (this.Servicos.ContainsKey(nome))
            {
                throw new Exception(String.Format("Já foi adicionado um manipulador para {0}", nome));
            }

            if (!this.Servicos.ContainsKey(nome))
            {
                if (!tipo.IsSubclassOf(typeof(BaseComunicacaoServidor)))
                {
                    throw new Exception(String.Format("O tipo não é suportado, ele deve herda de {0}", typeof(BaseComunicacaoServidor).Name));
                }
                this.Servicos.Add(nome, tipo);
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

#if NET50

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.Configure<IISServerOptions>(options =>
            //{
            //    options.AllowSynchronousIO = false;
            //});
            //services.AddResponseCompression();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {

#if DEBUG
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
#endif

            if (AplicacaoSnebur.Atual is AplicacaoSneburAspNetCore aplicacao)
            {
                aplicacao.ConfigureHttpContextAccessor(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>(), serviceProvider);
            }
            else
            {
                throw new Erro("A aplicacao AplicacaoSneburAspNetCore não foi inicializada");
            }

            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();
            //app.UseRouting();
            //app.UseMiddleware<Manipulador>();
            //app.UseDefaultFiles();
            //app.UseStaticFiles();
            //app.UseResponseCompression();

            app.Use(async (context, next) =>
            {
                await next.Invoke();
            });

            app.Run(async context =>
            {
                this.AntesProcessarRequisicao(context);
                await this.ProcessarRequisicaoAsync(context);
            });
        }

        protected virtual void AntesProcessarRequisicao(HttpContext context)
        {

        }

        public static void Inicializar<T>(AplicacaoSneburAspNetCore aplicacao) where T : BaseManipuladorRequisicao
        {
            aplicacao.Inicializar();


            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json");

            var configuracao = builder.Build();
            aplicacao.Configure(configuracao);



            var webHostBuilder = Host.CreateDefaultBuilder();
            webHostBuilder.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<T>();
            });
            var servidorWeb = webHostBuilder.Build();
            servidorWeb.Run();

            //CreateHostBuilder(args).Build().Run();
        }
#endif

        #region Métodos abstratos

        protected abstract string RetornarIdentificadorProprietario(HttpRequest httpRequest);

        protected abstract bool IsAplicacaoAutorizada(HttpRequest request);

        #endregion

        public void Dispose()
        {
        }

    }
}
