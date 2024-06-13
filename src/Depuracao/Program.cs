#if NET6_0_OR_GREATER

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snebur
{
    public class Program
    {
        private static string[] ArquivosAutorizados = new string[] { "download",
                                                                     ".json",
                                                                     "login.html",
                                                                     "debug.aspx" };

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            var pathBuild = Path.Combine(builder.Environment.ContentRootPath, "build");
            var staticFileOptions = new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(pathBuild),
                RequestPath = "/build"
            };
            app.UseStaticFiles(staticFileOptions);

            var defaultFileOptions = new DefaultFilesOptions();
            defaultFileOptions.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles();

            app.Run(async context =>
            {
                var request = context.Request;
                var path = request.Path.ToString().ToLower();
                if (ArquivosAutorizados.Any(x => path.Contains(x)))
                {
                    return;
                }
                 
                if (request.Method == "GET" && !IsFileName(path))
                {
                    var response = context.Response;
                    var vsPort = GetPortaVsDepuracao();

                    response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
                    response.Headers.Append("Pragma", "no-cache");
                    response.Headers.Append("Expires", "0");
                    response.Headers.Append("vs-depuracao-porta", vsPort.ToString());

                    response.ContentType = "text/html; charset=utf-8";

                    var caminhoIndexHtml = Path.Combine(Directory.GetCurrentDirectory(), "index.html");
                    if (caminhoIndexHtml != null && File.Exists(caminhoIndexHtml))
                    {
                        var conteudo = File.ReadAllText(caminhoIndexHtml, Encoding.UTF8);
                        var debugVersion = DateTime.Now.ToString("yyyy-MM-dd.HH_mm.ss.ffff");
                        conteudo = conteudo.Replace("[[VERSAO]]", debugVersion);
                        await response.WriteAsync(conteudo);
                    }
                }
            });
            app.Run();
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

        static int GetPortaVsDepuracao()
        {
            try
            {
                var caminho = Path.Combine(Directory.GetCurrentDirectory(), "vs-depuracao-porta.info");
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
    }

    public class VsDepuracaoMiddleware
    {
        private readonly RequestDelegate _next;

        public VsDepuracaoMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Adicione o valor da variável no cabeçalho HTTP
            context.Response.Headers.Remove("vs-depuracao-porta");
            var caminho = Path.Combine(Directory.GetCurrentDirectory(), "vs-depuracao-porta.info");
            if (File.Exists(caminho))
            {
                try
                {
                    var vsPort = File.ReadAllText(caminho);
                    context.Response.Headers.Append("vs-depuracao-porta", vsPort);
                }
                catch
                {

                }
            }
            await this._next(context);
        }
    }
}

#endif