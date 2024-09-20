#if NET6_0_OR_GREATER

// Don't remove this using namespace declaration
 using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Snebur
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder.Services);

            var pathCert = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "../../../sigi.pfx"));
            if (File.Exists(pathCert))
            {
                builder.WebHost.ConfigureKestrel(options =>
                {

                    options.ListenAnyIP(44380, listenOptions =>
                    {
                        listenOptions.UseHttps(pathCert, "zyon@3319");
                    });
                });
            }

           
            var app = builder.Build();
            var caminhoAplicacao = builder.Environment.ContentRootPath;

            Configure(app, app.Environment);

            app.Run(async context =>
            {
                try
                {
                    await TryProcessarAsync(context, caminhoAplicacao);
                }
                catch 
                { }
            });
            app.Run();
        }

        private static async Task TryProcessarAsync(HttpContext context, string caminhoAplicacao)
        {
            try
            {
                await ProcessarAsync(context, caminhoAplicacao);
            }
            catch (Exception ex)
            {
                await context.Response.WriteAsync(ex.Message);
            }
        }
        private static async Task ProcessarAsync(HttpContext context, string caminhoAplicacao)
        {
            var request = context.Request;
            var path = request.Path.ToString().ToLower();

            if (path == "/vs-porta-depuracao")
            {
                var vsPort = GetPortaVsDepuracao(caminhoAplicacao);
                await context.Response.WriteAsync(vsPort.ToString());
                return;
            }

            var response = context.Response;
            var extensao = Path.GetExtension(path);

            if (IsArquivoSistema(extensao))
            {
                var fullPath = CombinePaths(caminhoAplicacao, path);
                var melhorMimeType = GetMimeType(extensao);
                await ReponderArquivoAsync(response,
                                           fullPath,
                                           melhorMimeType);
                return;
            }

            if (request.Method == "GET" && !IsFileName(path))
            {
                var caminhoIndexHtml = Path.Combine(caminhoAplicacao, "wwwroot/index.html");
                if (caminhoIndexHtml != null && File.Exists(caminhoIndexHtml))
                {
                    await ReponderArquivoHtmlAsync(response, caminhoIndexHtml);
                }
            }
        }

        

        static bool IsArquivoSistema(string extensao)
        {
            return extensao == ".shtml" ||
                   extensao == ".scss" ||
                   extensao == ".map" ||
                   extensao == ".json" ||
                   extensao == ".js" ||
                   extensao == ".ts";
        }

        static string GetMimeType(string extensao)
        {
            switch (extensao)
            {
                case ".shtml":
                    return "text/html";
                case ".scss":
                    return "text/css";
                case ".ts":
                    return "text/javascript";
                case ".map":
                    return "application/json";
                default:

                    var provider = new FileExtensionContentTypeProvider();
                    if (provider.TryGetContentType(extensao, out string? mimeType))
                    {
                        return mimeType;
                    }
                    return "application/octet-stream";
            }
        }

        static Task ReponderArquivoHtmlAsync(HttpResponse response,
                                                   string caminhoHtml)
        {
            return ReponderArquivoAsync(response, caminhoHtml, "text/html");
        }

        static async Task ReponderArquivoAsync(HttpResponse response,
                                               string caminhoArquivo,
                                               string mimeType)
        {
            if (caminhoArquivo != null && File.Exists(caminhoArquivo))
            {
                response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
                response.Headers.Append("Pragma", "no-cache");
                response.Headers.Append("Expires", "0");


                response.ContentType = $"{mimeType}; charset=utf-8";
                var conteudo = File.ReadAllText(caminhoArquivo, Encoding.UTF8);
                var debugVersion = DateTime.Now.ToString("yyyy-MM-dd.HH_mm.ss.ffff");
                conteudo = conteudo.Replace("[[VERSAO]]", debugVersion);
                await response.WriteAsync(conteudo);
                await response.CompleteAsync();
            }
            else
            {
                response.StatusCode = 404;
                await response.WriteAsync($"Arquivo não encontrado: {Path.GetFileName(caminhoArquivo)} ");
            }
        }

        static bool IsFileName(string path)
        {
            var nomeArquivo = Path.GetFileName(path);
            if (String.IsNullOrEmpty(nomeArquivo))
            {
                return false;
            }
            return nomeArquivo.IndexOf(".") > 0;
        }

        static int GetPortaVsDepuracao(string caminhoAplicacao)
        {
            try
            {
                var caminho = Path.Combine(caminhoAplicacao, "vs-depuracao-porta.info");
                if (File.Exists(caminho))
                {

                    var vsPort = File.ReadAllText(caminho);
                    if (Int32.TryParse(vsPort, out int porta))
                    {
                        return porta;
                    }
                }

            }
            catch { }
            return 0;
        }

        public static string CombinePaths(string path1, string path2)
        {
            if (Path.IsPathRooted(path2))
            {
                path2 = path2.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }
            return Path.Combine(path1, path2);
        }

        public static void ConfigureServices(IServiceCollection services)
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

        private static void Configure(WebApplication app, IWebHostEnvironment environment)
        {
            //var pathBuild = Path.Combine(caminhoAplicacao, "build");
            //var pathApresentacao = Path.Combine(caminhoAplicacao, "apresentacao");

            app.UseDeveloperExceptionPage();
            app.UseCors("CrossDomainAll");

            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(pathBuild),
            //    RequestPath = "/build"
            //});

            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(pathBuild),
            //    RequestPath = "/apresentacao",
            //    ContentTypeProvider = new FileExtensionContentTypeProvider
            //    {
            //        Mappings = { [".shtml"] = "text/html" }
            //    }
            //});

            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = new FileExtensionContentTypeProvider
                {
                    Mappings = { [".shtml"] = "text/html" }
                }
            });

            //var defaultFileOptions = new DefaultFilesOptions();
            //defaultFileOptions.DefaultFileNames.Add("index.html");
            //app.UseDefaultFiles(defaultFileOptions);
        }
    }

    //public class VsDepuracaoMiddleware
    //{
    //    private readonly RequestDelegate _next;

    //    public VsDepuracaoMiddleware(RequestDelegate next)
    //    {
    //        this._next = next;
    //    }

    //    public async Task InvokeAsync(HttpContext context)
    //    {
    //        // Adicione o valor da variável no cabeçalho HTTP
    //        context.Response.Headers.Remove("vs-depuracao-porta");
    //        var caminho = Path.Combine(Directory.GetCurrentDirectory(), "vs-depuracao-porta.info");
    //        if (File.Exists(caminho))
    //        {
    //            try
    //            {
    //                var vsPort = File.ReadAllText(caminho);
    //                context.Response.Headers.Append("vs-depuracao-porta", vsPort);
    //            }
    //            catch
    //            {

    //            }
    //        }
    //        await this._next(context);
    //    }
    //}
}

#endif