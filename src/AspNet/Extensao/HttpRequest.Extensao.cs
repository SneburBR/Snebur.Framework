using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Net.Http.Headers;

namespace Snebur;
public static class HttpRequestExtensao
{
    public static Uri RetornarUrlRequisicao(this HttpRequest request)
    {
        return new Uri($"{request.Scheme}://{request.Host.Host}{request.GetEncodedPathAndQuery()}");

    }

    public static string GetMethod(this HttpRequest request)
    {
        return request.Method;
    }

    public static IFormFileCollection GetFiles(this HttpRequest request)
    {
        return request.Form.Files;
    }

    public static string? GetValue(this HttpRequest request, string key)
    {

        if (request.HasFormContentType)
        {
            if (request.Form.TryGetValue(key, out var formValue))
            {
                if (formValue.Count == 1)
                {
                    return formValue.Single();
                }
                return formValue.ToString();
            }
        }

        if (request.Query.TryGetValue(key, out var value))
        {
            if (value.Count == 1)
            {
                return value.Single();
            }
            return value.ToString();
        }
        return null;

    }

    public static Uri GetUrl(this HttpRequest request)
    {
#if NET6_0_OR_GREATER
        return new Uri($"{request.Scheme}://{request.Host.Host}{request.Path}");
#else
        return request.UrlReferrer ?? request.Url;
#endif
    }

    public static string GetHost(this HttpRequest request)
    {
#if NET6_0_OR_GREATER
        return request.Host.Host;
#else
        return request.UrlReferrer?.Host ?? request.Url.Host;
#endif
    }

#if NET6_0_OR_GREATER

    public static string UserAgent(this HttpRequest request)
    {
        return request.Headers[HeaderNames.UserAgent].ToString();
    }

    public static void Write(this Stream stream, string texto)
    {
        stream.Write(Encoding.UTF8.GetBytes(texto));
    }

    public static async Task CompleteRequestAsync(this HttpContext httpContext)
    {
        var response = httpContext.Response;
        await response.CompleteAsync();

        //httpContext.Response.Body.Close();
    }
#endif
}
