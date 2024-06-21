#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Snebur.Dominio;
using System.Net;

namespace Snebur.Comunicacao
{
    public class Startup
    {
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
                            .SetBasePath(diretorioBase)
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

            var caminhoAplicacao = builder.Environment.ContentRootPath;
            aplicacaoSnebur.Inicializar();

            Configure(app, app.Environment, isWebSocket);

            //app.UseStaticFiles();

            _manipulador = Activator.CreateInstance(typeof(T), new object[] { caminhoAplicacao}) as BaseManipuladorRequisicao;

            app.Run(async context =>
            {
                //using (var manipulador = Activator.CreateInstance<T>())
                //{
                try
                {
                    context.Items.Add(ConstantesItensRequsicao.CAMINHO_APLICACAO, caminhoAplicacao);
                    _manipulador.AntesProcessarRequisicao(context);
                    await _manipulador.ProcessarRequisicaoAsync(context);
                }
                catch(Exception ex)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync("Erro interno: " + ex.Message);
                }
                finally
                {
                    _manipulador.DepoisProcessarRequisicao(context);
                }
                
                //}
            });
            app.Run();

            return app;

            //CreateHostBuilder(args).Build().Run();
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
     
            if (env.IsDevelopment() ||  env.IsStaging())
            {
                app.UseDeveloperExceptionPage();
            }
#endif
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            //app.UseDeveloperExceptionPage();

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
    }
}


#endif