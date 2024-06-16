
#if NET6_0_OR_GREATER

namespace Snebur.Comunicacao
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting.Server;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Snebur.Utilidade;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.WebSockets;
    using System.Reflection;
    using System.Reflection.PortableExecutable;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;

    public abstract partial class BaseManipuladorRequisicao : IHttpModule, IDisposable
    {
        public bool IsWebSocket { get; protected set; }

        private static BaseManipuladorRequisicao _manipulador;
        public static WebApplication Inicializar<T>(AplicacaoSneburAspNet aplicacaoSnebur,
                                                    bool isWebSocket = false) where T : BaseManipuladorRequisicao
        {

            var diretorioBase = Directory.GetCurrentDirectory();
            if (!Directory.Exists(diretorioBase))
            {
                throw new DirectoryNotFoundException(diretorioBase);
            }

            var configBuilder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json");

            var configuracao = configBuilder.Build();
            aplicacaoSnebur.Configure(configuracao);

            //
            //var webHostBuilder = Host.CreateDefaultBuilder();
            //webHostBuilder.ConfigureWebHostDefaults(webBuilder =>
            //{
            //    webBuilder.UseStartup<T>();
            //});
            //var servidorWeb = webHostBuilder.Build();
            //servidorWeb.Run();

            var builder = WebApplication.CreateBuilder();
            builder.Configuration.AddConfiguration(configuracao);
            //builder.WebHost.ConfigureKestrel(serverOptions =>
            //{
            //    serverOptions.AllowSynchronousIO = true;
            //});

            ConfigureServicesInterno(builder.Services);
 
            if (isWebSocket)
            {
                builder.Services.AddSignalR();
            }

            var app = builder.Build();
            

            aplicacaoSnebur.Inicializar();

            Configure(app, app.Environment, isWebSocket);

            //app.UseStaticFiles();

            _manipulador = Activator.CreateInstance<T>();

            app.Run(async context =>
            {
                using (var manipulador = Activator.CreateInstance<T>())
                {
                    manipulador.AntesProcessarRequisicao(context);
                    await manipulador.ProcessarRequisicaoAsync(context);
                }
            });
            app.Run();

            return app;

            //CreateHostBuilder(args).Build().Run();
        }

        public IConfigurationRoot Configuration => throw new NotImplementedException();

        public void ConfigureServices(IServiceCollection services)
        {
            BaseManipuladorRequisicao.ConfigureServicesInterno(services);
        }

        public static void ConfigureServicesInterno(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpContextAccessor();

            services.AddCors(options =>
            {
                options.AddPolicy("CrossDomainAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });
            //services.AddControllersWithViews();
            //services.AddControllers();
        }

        public static void Configure(WebApplication app,
                                    IWebHostEnvironment env,
                                    bool isWebSocket)
        {

#if DEBUG
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }
#endif
            app.UseDeveloperExceptionPage();

            if (env.IsProduction())
            {
                //app.UseExceptionHandler("/Error");
                app.UseHsts();
                //app.UseResponseCompression();
            }

            app.UseCors("CrossDomainAll");

            if (AplicacaoSnebur.Atual is AplicacaoSneburAspNet aplicacao)
            {
                var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
                aplicacao.ConfigureHttpContextAccessor(httpContextAccessor);
            }
            else
            {
                throw new Erro("A aplicação AplicacaoSneburAspNetCore não foi inicializada");
            }

            if (isWebSocket)
            {
                var webSocketOptions = new WebSocketOptions
                {
                    KeepAliveInterval = TimeSpan.FromMinutes(2)
                };
                app.UseWebSockets(webSocketOptions);

                app.Use(async (context, next) =>
                {
                    if (context.Request.Path == "/ws")
                    {
                        if (context.WebSockets.IsWebSocketRequest)
                        {
                            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                            {
                                //await Echo(webSocket);
                                throw new NotImplementedException();
                            }
                        }
                        else
                        {
                            context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        }
                    }

                    await next.Invoke();
                });
            }



            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();
            //app.UseRouting();
            //app.UseMiddleware<Manipulador>();
            //app.UseDefaultFiles();
            //app.UseStaticFiles();
            //app.UseResponseCompression();

        }

        public async Task ProcessarRequisicaoAsync(HttpContext context)
        {
            if (CrossDomainUtil.VerificarContratoCrossDomain(context))
            {
                await context.CompleteRequestAsync();
                return;
            }
            this.AntesProcessarRequisicao(context);

            var request = context.Request;
            var response = context.Response;

            if (DebugUtil.IsAttached)
            {
                await this.TesteHttpRequestAsync(context);
            }

            if (await this.IsExecutarServicoAsync(context))
            {
                response.StatusCode = 0;
                try
                {
                    await this.ExecutarServicoAsync(context);
                    //Chamadas do cross domain do ajax serão implementas aqui
                }
                catch (Exception ex)
                {

                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    if (DebugUtil.IsAttached)
                    {
                        throw;
                    }
                    LogUtil.ErroAsync(ex);
                }
                finally
                {
                    if (response.StatusCode == 0)
                    {
                        response.StatusCode = (int)HttpStatusCode.Conflict;
                    }
                    await context.CompleteRequestAsync();
                }
            }
        }

        private async Task TesteHttpRequestAsync(HttpContext context)
        {
            await Task.Delay(500);
            ThreadUtil.ExecutarStaAsync(() =>
            {
                var httpCtx = AplicacaoSnebur.Atual.AspNet.GetHttpContext<HttpContext>();
                if (httpCtx != context)
                {
                    throw new Exception("Falha ao obter o HttpContext");
                }
            }, "ThreadTESTEHttpContext");
            await Task.Delay(500);
            await Task.Factory.StartNew(() =>
             {
                 var httpCtx = AplicacaoSnebur.Atual.AspNet.GetHttpContext<HttpContext>();
                 if (httpCtx != context)
                 {
                     throw new Exception("Falha ao obter o HttpContext");
                 }
             });
            await Task.Delay(500);
        }

        private async Task ExecutarServicoAsync(HttpContext httpContext)
        {
            var request = httpContext.Request;

            if (this.IsRequicaoValida(request, true))
            {
                var identificadorProprietario = this.RetornarIdentificadorProprietario(request);
                var nomeManipulador = request.Headers[ParametrosComunicacao.MANIPULADOR];
                var tipoManipulador = this.RetornarTipoServico(nomeManipulador);
                if (tipoManipulador == null)
                {
                    this.NotificarServicoNaoEncontado(httpContext, nomeManipulador);
                    return;
                }
                using (var servico = (BaseComunicacaoServidor)Activator.CreateInstance(tipoManipulador))
                {
                    try
                    {
                        servico.IdentificadorProprietario = identificadorProprietario;
                        await servico.ProcessRequestAsync(httpContext);
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

        private async Task<bool> IsExecutarServicoAsync(HttpContext httpContext)
        {
            var request = httpContext.Request;
            var response = httpContext.Response;

            var caminho = Path.GetFileName(request.RetornarUrlRequisicao().LocalPath).ToLower();

            if (CrossDomainUtil.VerificarContratoCrossDomain(httpContext))
            {
                await httpContext.CompleteRequestAsync();
                return false;
            }

            if (caminho == "/" || caminho == "")
            {
                await this.ResponderAgoraAsync(response);
                await httpContext.CompleteRequestAsync();
                return false;
            }

            if (caminho.Equals("agora", StringComparison.InvariantCultureIgnoreCase))
            {
                await this.ResponderAgoraAsync(response);
                await httpContext.CompleteRequestAsync();
                return false;
            }

            if (caminho.Equals("ping", StringComparison.InvariantCultureIgnoreCase))
            {
                await response.WriteAsync("True", Encoding.UTF8);
                await httpContext.CompleteRequestAsync();
                return false;
            }

            var caminhoManipulador = Path.GetFileNameWithoutExtension(caminho);
            if (this.ManipuladoresGenericos.ContainsKey(caminhoManipulador))
            {
                var isValidarToken = this.ManipuladoresGenericos[caminhoManipulador].isValidarToken;
                if (isValidarToken && !this.IsRequicaoValida(request, false))
                {
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await httpContext.CompleteRequestAsync();
                    return false;
                }

                await this.ExecutarManipuladorGenericoAsync(httpContext, caminhoManipulador);
                await httpContext.CompleteRequestAsync();
                return false;
            }

            if (this.ArquivosAutorizados.ContainsKey(caminho))
            {
                var isIgnorarValidacaoTokenAplicacao = this.ArquivosAutorizados[caminho];
                if (!isIgnorarValidacaoTokenAplicacao)
                {
                    if (!this.IsRequicaoValida(request, false))
                    {
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        await httpContext.CompleteRequestAsync();
                        return false;
                    }
                }
                return false;
            }

            if ((request.Headers.ContainsKey(ParametrosComunicacao.TOKEN) &&
                !request.Headers.ContainsKey("TOKEN")))
            {
                throw new Exception("Invalida case insensitive");
                //await httpContext.CompleteRequestAsync();
                //return false;
            }

            if (!request.Headers.ContainsKey(ParametrosComunicacao.TOKEN) ||
                !request.Headers.ContainsKey(ParametrosComunicacao.MANIPULADOR))
            {

                LogUtil.SegurancaAsync($"A URL '{request.RetornarUrlRequisicao().AbsoluteUri}' foi chamada incorretamente.", Servicos.EnumTipoLogSeguranca.CabecalhoInvalido);
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                await httpContext.CompleteRequestAsync();
                return false;
            }
            return true;
        }

        private async Task ResponderAgoraAsync(HttpResponse response)
        {
            var agora = DateTime.UtcNow.AddSeconds(-10);
            await this.ResponderAsync(response, agora.Ticks.ToString());
        }

        private async Task ResponderAsync(HttpResponse response, string conteuto)
        {
            response.ContentType = "text/text; charset=UTF-8";
            if (!response.HasStarted)
            {
                response.StatusCode = 200;
            }
            await response.WriteAsync(conteuto, Encoding.UTF8);
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

        private static async Task Echo(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];

            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);


            while (!receiveResult.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(
                    new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                    receiveResult.MessageType,
                    receiveResult.EndOfMessage,
                    CancellationToken.None);

                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);
        }
    }


    public class AshxHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public AshxHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Value.EndsWith(".ashx"))
            {
                await HandleAshxRequest(context);
            }
            else
            {
                await _next(context);
            }
        }

        private Task HandleAshxRequest(HttpContext context)
        {
            // Lógica do seu handler .ashx aqui
            context.Response.ContentType = "text/plain";
            return context.Response.WriteAsync("Hello from .ashx handler!");
        }
    }

    public static class AshxHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseAshxHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AshxHandlerMiddleware>();
        }
    }

}


#endif