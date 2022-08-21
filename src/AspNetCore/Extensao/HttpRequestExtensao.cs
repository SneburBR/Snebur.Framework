using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Text;

//https://docs.microsoft.com/pt-br/aspnet/core/migration/http-modules?view=aspnetcore-5.0

namespace Microsoft.AspNetCore.Http
{
    public static class HttpRequestExtensao
    {
        public static Uri RetornarUrlRequisicao(this HttpRequest request)
        {
            return request.GetTypedHeaders()?.Referer ??
                   new Uri($"{request.Scheme}://{request.Host.Host}{ request.GetEncodedPathAndQuery()}");
        }
        public static Uri Url(this HttpRequest request)
        {
            var header = request.GetTypedHeaders();
            return header.Referer;
        }

        public static string UserAgent(this HttpRequest request)
        {
            string userAgent = request.Headers[HeaderNames.UserAgent].ToString();
            return userAgent;
        }

        public static string GetValue(this IHeaderDictionary cabecalho, string chave)
        {
            if (cabecalho.TryGetValue(chave, out var item))
            {
                if (item.Count == 1)
                {
                    return item.ToString();
                }
                throw new Erro($"Existe mais de um item no cabeçalho da requisação para chave {chave}");
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

        public static void Write(this Stream stream, string texto)
        {
            stream.Write(Encoding.UTF8.GetBytes(texto));
        }

        public static void CompleteRequest(this HttpContext httpContext)
        {
            //httpContext.Response.Body.Close();
        }
    }


}