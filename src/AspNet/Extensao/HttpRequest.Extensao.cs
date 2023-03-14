using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NET7_0
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Net.Http.Headers;
#endif

namespace System.Web
{
    public static class HttpRequestExtensao
    {
        public static Uri RetornarUrlRequisicao(this HttpRequest request)
        {
#if NET7_0
            return request.GetTypedHeaders()?.Referer ??
                   new Uri($"{request.Scheme}://{request.Host.Host}{request.GetEncodedPathAndQuery()}");
#else
            return request.Url ??
                   request.UrlReferrer;
#endif

        }

        public static string GetQueryStringValue(this HttpRequest request, string key)
        {
#if NET7_0
            if( request.Query.TryGetValue(key, out var value))
            {
                if(value.Count == 1)
                {
                    return value.Single();
                }
                return value.ToString();
            }
            return null;

#else
            return request[key];
#endif
        }

#if NET7_0
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
#endif
    }

}
