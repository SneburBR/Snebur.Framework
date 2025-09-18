using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Snebur.Net;

public class ModernHttpClientProxy : HttpClientProxy
{
    private readonly HttpClient _client;
    private readonly Uri _uri;
    private MemoryStream? _requestBodyStream;

    public ModernHttpClientProxy(string url)
    {
        var handler = new HttpClientHandler
        {
            UseProxy = false,
            Proxy = null,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };

        this._client = new HttpClient(handler);
        this._uri = new Uri(url);
    }

    public override Stream GetRequestStream()
    {
        if (_requestBodyStream == null)
        {
            _requestBodyStream = ContentLength > 0 ? new MemoryStream(ContentLength) : new MemoryStream();
        }
        return _requestBodyStream;
    }

    public override BaseHttpWebResponseProxy GetResponse()
    {
        if (Timeout > 0)
        {
            _client.Timeout = TimeSpan.FromMilliseconds(Timeout);
        }

        var method = new HttpMethod(string.IsNullOrWhiteSpace(Method) ? "GET" : Method);
        var request = new HttpRequestMessage(method, _uri);

        // Corpo da requisição, quando houver
        if (_requestBodyStream != null)
        {
            var bytes = _requestBodyStream.ToArray(); // desacopla do stream original
            var content = new ByteArrayContent(bytes);

            if (!string.IsNullOrWhiteSpace(ContentType))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue(ContentType);
            }

            if (ContentLength > 0)
            {
                content.Headers.ContentLength = ContentLength;
            }

            request.Content = content;
        }
        else
        {
            // Alguns métodos esperam content, mesmo vazio
            if (string.Equals(Method, "POST", StringComparison.OrdinalIgnoreCase)
                || string.Equals(Method, "PUT", StringComparison.OrdinalIgnoreCase)
                || string.Equals(Method, "PATCH", StringComparison.OrdinalIgnoreCase))
            {
                var content = new ByteArrayContent(Array.Empty<byte>());
                if (!string.IsNullOrWhiteSpace(ContentType))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue(ContentType);
                }
                request.Content = content;
            }
        }

        // Cabeçalhos
        foreach (var key in Headers.AllKeys)
        {
            var values = Headers.GetValues(key);
            if (values == null) continue;

            if (!request.Headers.TryAddWithoutValidation(key, values))
            {
                if (request.Content == null)
                {
                    request.Content = new ByteArrayContent(Array.Empty<byte>());
                }
                request.Content.Headers.TryAddWithoutValidation(key, values);
            }
        }

        // Envia e retorna um proxy compatível
        var response = _client
            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        var proxy = new HttpResponseMessageProxy(response)
        {
            StatusCode = response.StatusCode
        };
        return proxy;
    }
}
