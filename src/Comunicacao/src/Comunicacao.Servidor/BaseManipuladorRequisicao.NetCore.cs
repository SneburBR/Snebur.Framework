﻿
#if NetCore
using Microsoft.AspNetCore.Http;

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
    using Snebur.AspNetCore;
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
        public static WebApplication Inicializar<T>(AplicacaoSneburAspNet aplicacaoSnebur,
                                                    bool isWebSocket) where T : BaseManipuladorRequisicao
        {

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

            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.Services.AddHttpContextAccessor();


            if (isWebSocket)
            {
                builder.Services.AddSignalR();
            }

            var app = builder.Build();
            aplicacaoSnebur.Inicializar();

            Configure(app, app.Environment, isWebSocket);

            //app.Run(async context =>
            //{
            //    using (var manipulador = Activator.CreateInstance<T>())
            //    {
            //        manipulador.AntesProcessarRequisicao(context);
            //        await manipulador.ProcessarRequisicaoAsync(context);
            //    }
            //});

            return app;



            //CreateHostBuilder(args).Build().Run();
        }

        public IConfigurationRoot Configuration => throw new NotImplementedException();

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public static void Configure(WebApplication app,
                                    IWebHostEnvironment env,
                                    bool isWebSocket)
        {

#if DEBUG
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
#endif
            if (isWebSocket)
            {
                var webSocketOptions = new WebSocketOptions
                {
                    KeepAliveInterval = TimeSpan.FromMinutes(2)
                };
                app.UseWebSockets(webSocketOptions);
            }

            if (AplicacaoSnebur.Atual is AplicacaoSneburAspNet aplicacao)
            {
                var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
                aplicacao.ConfigureHttpContextAccessor(httpContextAccessor);
            }
            else
            {
                throw new Erro("A aplicação AplicacaoSneburAspNetCore não foi inicializada");
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

        public async Task ProcessarRequisicaoAsync(HttpContext context)
        {
           if (CrossDomainUtil.VerificarContratoCrossDomain(httpContext))
            {
                await httpContext.CompleteRequestAsync();
                return false;
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


}


#endif