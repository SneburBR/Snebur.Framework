using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NET6_0_OR_GREATER
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
#if NET6_0_OR_GREATER
            return new Uri($"{request.Scheme}://{request.Host.Host}{request.GetEncodedPathAndQuery()}");
#else
            return request.Url ??
                   request.UrlReferrer;
#endif

        }

        public static string GetValue(this HttpRequest request, string key)
        {
#if NET6_0_OR_GREATER

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

#else
            return request[key];
#endif
        }

#if NET6_0_OR_GREATER
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
