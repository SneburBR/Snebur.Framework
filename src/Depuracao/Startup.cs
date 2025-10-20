#if NET6_0_OR_GREATER

// Don't remove this using namespace declaration
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Snebur;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder.Services);
        var pathRoot = builder.Environment.ContentRootPath;

        //var pathCert = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "../../../cert/sigi.pfx"));
        //if (File.Exists(pathCert))
        //{
        //    var port = GetPortApplicationUrl(pathRoot);
        //    builder.WebHost.ConfigureKestrel(options =>
        //    {
        //        options.ListenAnyIP(port, listenOptions =>
        //        {
        //            listenOptions.UseHttps(pathCert, "zyon@3319");
        //        });
        //    });
        //}

        var app = builder.Build();
        var caminhoAplicacao = builder.Environment.ContentRootPath;
        var builderPath = new FilePathBuiler(caminhoAplicacao);
        Configure(app, app.Environment);

        var handler = new StaticFileHandler(caminhoAplicacao);

        app.Use(async (context, next) =>
        {
            try
            {
                //await _lockAsync.WaitAsync();
                await handler.ProcessAsync(context);
            }
            catch (Exception ex)
            {
                Debugger.Break();
                await context.Response.WriteAsync(ex.Message);
            }
            finally
            {
                //_lockAsync.Release();
            }
            await next();
        });
        app.Run();
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

    public static string BuildFullPath(string applicationPath, string relativePath)
    {
        if (Path.IsPathRooted(relativePath))
        {
            relativePath = relativePath.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
        return Path.GetFullPath(Path.Combine(applicationPath, relativePath));
    }
}

public class StaticFileHandler
{
    private readonly string _applicationPath;
    private readonly FilePathBuiler _filePathBuiler;

    public StaticFileHandler(string applicationPath)
    {
        this._applicationPath = applicationPath;
        this._filePathBuiler = new FilePathBuiler(applicationPath);
    }

    public async Task ProcessAsync(HttpContext context)
    {
        var request = context.Request;
        var url = request.GetDisplayUrl();
        var path = request.Path.ToString();

        if (path == "/vs-porta-depuracao")
        {
            var vsPort = GetPortaVsDepuracao(this._applicationPath);
            await context.Response.WriteAsync(vsPort.ToString());
            return;
        }

        var response = context.Response;
        var extensao = Path.GetExtension(path);

        if (this.IsArquivoSistema(extensao))
        {
            if (extensao == ".map")
                Debugger.Break();

            var fullPath = this._filePathBuiler.Build(path);
            var melhorMimeType = this.GetMimeType(extensao);
            await this.ReponderArquivoAsync(response,
                                       fullPath,
                                       melhorMimeType);
            return;
        }
  
        if (request.Method == "GET" && !IsFileName(path))
        {
            var caminhoIndexHtml = Path.Combine(this._applicationPath, "wwwroot/index.html");
            if (caminhoIndexHtml is null || !File.Exists(caminhoIndexHtml))
            {
                throw new FileNotFoundException("Arquivo index.html não encontrado na pasta wwwroot.", caminhoIndexHtml);
            }
            await this.ReponderArquivoHtmlAsync(response, caminhoIndexHtml);
        }
    }

    private bool IsArquivoSistema(string extensao)
    {
        return extensao == ".shtml" ||
               extensao == ".scss" ||
               extensao == ".css" ||
               extensao == ".map" ||
               extensao == ".json" ||
               extensao == ".js" ||
               extensao == ".ts";
    }

    private string GetMimeType(string extensao)
    {
        switch (extensao)
        {
            case ".shtml":
                return "text/html";
            case ".scss":
            case ".css":
                return "text/css";
            case ".js":
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

    private Task ReponderArquivoHtmlAsync(HttpResponse response,
                                               string caminhoHtml)
    {
        return this.ReponderArquivoAsync(response, caminhoHtml, "text/html");
    }

    private async Task ReponderArquivoAsync(HttpResponse response,
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
            Debugger.Break();
            response.StatusCode = 404;
            await response.WriteAsync($"Arquivo não encontrado: {Path.GetFileName(caminhoArquivo)} ");
        }
    }

    private bool IsFileName(string path)
    {
        var nomeArquivo = Path.GetFileName(path);
        if (String.IsNullOrEmpty(nomeArquivo))
        {
            return false;
        }
        return nomeArquivo.IndexOf(".") > 0;
    }

    private int GetPortaVsDepuracao(string caminhoAplicacao)
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

    private int GetPortApplicationUrl(string path)
    {
        var lanchSettings = Path.Combine(path, "Properties/launchSettings.json");
        if (File.Exists(lanchSettings))
        {
            var conteudo = File.ReadAllText(lanchSettings);
            var index = conteudo.IndexOf("applicationUrl");
            if (index > 0)
            {
                conteudo = conteudo.Substring(index);

                var serach = "https://";
                index = conteudo.IndexOf(serach);
                conteudo = conteudo.Substring(index + serach.Length);
                index = conteudo.IndexOf(':');
                conteudo = conteudo.Substring(index + 1);

                index = Math.Min(conteudo.IndexOf(';'), conteudo.IndexOf('"'));

                int indexSemicolon = conteudo.IndexOf(';');
                int indexQuote = conteudo.IndexOf('"');

                // Handle cases where IndexOf returns -1
                if (indexSemicolon == -1) indexSemicolon = int.MaxValue;
                if (indexQuote == -1) indexQuote = int.MaxValue;

                index = Math.Min(indexSemicolon, indexQuote);

                var port = conteudo.Substring(0, index);
                if (Int32.TryParse(port, out int porta))
                {
                    return porta;
                }
            }
        }
        return 5001;
    }
}

public class FilePathBuiler
{
    private readonly string _applicationPath;
    private readonly DirectoryInfo _applicationDir;
    private readonly Dictionary<string, string> _baseDirectories = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _caches = new(StringComparer.OrdinalIgnoreCase);

    public FilePathBuiler(string applicationPath)
    {
        this._applicationPath = applicationPath;
        this._applicationDir = new DirectoryInfo(applicationPath);
        this.PopuleKnowsPaths();
    }
    public string Build(string relativePath)
    {
        if (this._caches.TryGetValue(relativePath, out var cachedPath))
        {
            return cachedPath;
        }
        var path = this.BuildInternal(relativePath);
        if(path is null)
            throw new FileNotFoundException(
                $"Não foi possível localizar o arquivo '{relativePath}' nos diretórios conhecidos.",
                relativePath);

        this._caches.TryAdd(relativePath, path);
        return path;
    }

    private string BuildInternal(string relativePath)
    {
        lock (this)
        {
            var fullPath = CombineFullPath(relativePath);
            if (File.Exists(fullPath))
            {
                return fullPath;
            }

            return this.BuildRelative(relativePath);
        }
    }

    private string BuildRelative(string relativePath)
    {
        var parts = relativePath
            .Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar], StringSplitOptions.RemoveEmptyEntries)
            .ToArray();

        var queue = new Queue<string>(parts);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (this._baseDirectories.TryGetValue(current, out var baseDir))
            {
                var relative = Path.Combine([.. queue]);
                var fullPath = this.CombineFullPath(baseDir, relative);
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }
        }

        var currentDir = this._applicationDir;
        while (currentDir is not null && currentDir.Exists)
        {
            var fullPath = this.CombineFullPath(currentDir.FullName, relativePath);
            if (File.Exists(fullPath))
            {
                return fullPath;
            }
            currentDir = currentDir.Parent;
        }

        throw new FileNotFoundException(
            $"Não foi possível localizar o arquivo '{relativePath}' nos diretórios conhecidos.",
            relativePath);
    }

    private string CombineFullPath(string relativePath)
    {
        return this.CombineFullPath(this._applicationPath, relativePath);
    }

    private string CombineFullPath(string basePath, string relativePath)
    {
        if (Path.IsPathRooted(relativePath))
        {
            relativePath = relativePath.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
        var wwwpath = Path.GetFullPath(Path.Combine(basePath, "wwwroot", relativePath));
        if (File.Exists(wwwpath))
            return wwwpath;

        var buildPath = Path.GetFullPath(Path.Combine(basePath, "build", relativePath));
        if (File.Exists(buildPath))
            return buildPath;

        return Path.GetFullPath(Path.Combine(basePath, relativePath));
    }

    private void PopuleKnowsPaths()
    {
        var knowsDirectory = new HashSet<string>(["Zyoncore", "SneburBR"], StringComparer.OrdinalIgnoreCase);
        var current = new DirectoryInfo(this._applicationPath);
        while (current != null)
        {
            if (knowsDirectory.Contains(current.Name))
            {
                var sneburBRPath = Path.Combine(current.Parent?.FullName!, "sneburbr");
                var zyoncorePath = Path.Combine(current.Parent?.FullName!, "zyoncore");

                if (!Directory.Exists(sneburBRPath))
                    throw new DirectoryNotFoundException(
                        "Não foi possível localizar o diretório conhecido SneburBR.");

                if (!Directory.Exists(zyoncorePath))
                    throw new DirectoryNotFoundException(
                        "Não foi possível localizar o diretório conhecido Zyoncore.");

                this._baseDirectories.Add("sneburbr", sneburBRPath);
                this._baseDirectories.Add("zyoncore", sneburBRPath);
                return;
            }
            current = current.Parent;
        }

        throw new InvalidOperationException(
            "Não foi possível localizar os diretórios conhecidos Zyoncore ou SneburBR.");
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

#endif