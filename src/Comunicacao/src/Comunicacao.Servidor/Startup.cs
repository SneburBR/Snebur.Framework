#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Snebur.Dominio;
using System;
using System.IO;
using System.Net;

namespace Snebur.Comunicacao
{
    public delegate void ConfigureHandler(WebApplication app, IWebHostEnvironment env);

    public class Startup
    {
        private static BaseManipuladorRequisicao _manipulador;

        public static event ConfigureHandler ConfigureHandler;

        public static WebApplication Inicializar<T>(AplicacaoSneburAspNet aplicacaoSnebur) where T : BaseManipuladorRequisicao
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
            aplicacaoSnebur.SetConfigure(configuracao);

            var builder = WebApplication.CreateBuilder();
             
            builder.Configuration.AddConfiguration(configuracao);

#if NET8_0_OR_GREATER
            aplicacaoSnebur.AddServices(builder, builder.WebHost, configuracao);
#endif

            builder.Services.AddLogging(logging =>
            {
                logging.AddConsole()
                       .AddDebug()
                       .AddEventSourceLogger();
            });

            ConfigureServicesInterno(builder.Services);

            var app = builder.Build();

            var caminhoAplicacao = builder.Environment.ContentRootPath;
            aplicacaoSnebur.Inicializar();

            Configure(app, app.Environment);

            aplicacaoSnebur.Configure(app, app.Environment);

            Startup.ConfigureHandler?.Invoke(app, app.Environment);

            _manipulador = Activator.CreateInstance(typeof(T), new object[] { caminhoAplicacao }) as BaseManipuladorRequisicao;

            app.Use(async (context, next) =>
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<Startup>>();
                logger.LogInformation("Handling request: {Path}", context.Request.Path);
                //using (var manipulador = Activator.CreateInstance<T>())
                //{
                try
                {
                    context.Items.Add(ConstantesItensRequsicao.CAMINHO_APLICACAO, caminhoAplicacao);
                    _manipulador.AntesProcessarRequisicao(context);
                    await _manipulador.ProcessarRequisicaoAsync(context);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Erro interno: {Message}", ex.Message);

                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "text/plain";

                    await context.Response.WriteAsync($"Erro interno: {ex.Message}");
                }
                finally
                {
                    _manipulador.DepoisProcessarRequisicao(context);
                }
                await next();

                //}
            });
            app.Run();

            return app;

            //CreateHostBuilder(args).Build().Run();
        }
         
        public static void ConfigureServicesInterno(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            //services.AddScoped<IUserSessionService, UserSessionService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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
        }

        public static void Configure(IApplicationBuilder app,
                                     IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseDeveloperExceptionPage();
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
                var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>()
                    ?? throw new Erro("HttpContextAccessor não foi configurado"); ;

                aplicacao.ConfigureHttpContextAccessor(httpContextAccessor);
            }
            else
            {
                throw new Erro("A aplicação AplicacaoSneburAspNetCore não foi inicializada");
            }
        }

     
    }
}

#endif