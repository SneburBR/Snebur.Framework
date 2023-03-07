using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

//https://docs.microsoft.com/pt-br/aspnet/core/migration/http-modules?view=aspnetcore-5.0

namespace Snebur.AspNetCore
{
    public static class HttpRequestExtensao
    {
        public static Uri RetornarUrlRequisicao(this HttpRequest request)
        {
            return request.GetTypedHeaders()?.Referer ??
                   new Uri($"{request.Scheme}://{request.Host.Host}{request.GetEncodedPathAndQuery()}");
        }
        public static Uri Url(this HttpRequest request)
        {
            var header = request.GetTypedHeaders();
            return header.Referer;
        }

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
    }

    public static class IHeaderDictionaryExtensao
    {
        public static string GetValue(this IHeaderDictionary cabecalho, string chave)
        {
            if (cabecalho.TryGetValue(chave, out var item))
            {
                if (item.Count == 1)
                {
                    return item.ToString();
                }
                throw new Erro($"Existe mais de um item no cabeçalho da requisição para chave {chave}");
            }
            return null;
        }

        public static string[] GetValues(this IHeaderDictionary cabecalho, string chave)
        {
            if (cabecalho.TryGetValue(chave, out var item))
            {
                return item.ToArray();
            }
            return null;
        }
    }


}