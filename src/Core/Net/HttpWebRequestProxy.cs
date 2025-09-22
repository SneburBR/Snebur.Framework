using System.IO;
using System.Net;

namespace Snebur.Net;

public class HttpWebRequestProxy : HttpClientProxy
{
    private readonly HttpWebRequest _request;
    private HttpWebResponseProxy? _httpWebResponseProxy;
    private bool _applied = false;

    public HttpWebRequestProxy(string url)
    {
#pragma warning disable SYSLIB0014
        this._request = (HttpWebRequest)WebRequest.Create(url);
        this._request.Proxy = null;
#pragma warning restore SYSLIB0014
    }

    public override Stream GetRequestStream()
    {
        ApplyRequestConfig();
        return this._request.GetRequestStream();
    }

    public override BaseHttpWebResponseProxy GetResponse()
    {
        return this._httpWebResponseProxy ??= new HttpWebResponseProxy((HttpWebResponse)this._request.GetResponse());
    }

    private void ApplyRequestConfig()
    {
        if (_applied) return;
        _applied = true;

        // Propriedades básicas
        if (!string.IsNullOrWhiteSpace(Method))
        {
            _request.Method = Method;
        }

        if (Timeout > 0)
        {
            _request.Timeout = Timeout;
        }

        if (!string.IsNullOrWhiteSpace(ContentType))
        {
            _request.ContentType = ContentType;
        }

        if (ContentLength > 0)
        {
            // Só defina ContentLength se você vai escrever corpo
            // POST, PUT, PATCH geralmente têm corpo
            _request.ContentLength = ContentLength;
        }

        // Mapeamento de headers
        // Alguns são restritos, devem ser setados por propriedades
        foreach (var key in Headers.AllKeys)
        {
            var values = Headers.GetValues(key);
            if (values == null || values.Length == 0) continue;

            var name = key?.Trim();
            if (string.IsNullOrEmpty(name)) continue;

            var value = string.Join(", ", values);

            switch (name.ToLowerInvariant())
            {
                case "accept":
                    _request.Accept = value;
                    break;

                case "connection":
                    // Não é possível setar Connection diretamente
                    // Ajuste KeepAlive como alternativa mínima
                    if (value.IndexOf("close", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        _request.KeepAlive = false;
                    }
                    break;

                case "content-type":
                    // Já mapeado em ContentType
                    _request.ContentType = value;
                    break;

                case "content-length":
                    if (long.TryParse(value, out var cl) && cl >= 0)
                    {
                        _request.ContentLength = cl;
                    }
                    break;

                case "expect":
                    _request.Expect = value;
                    break;

                case "date":
                    // Header restrito, ignorar
                    break;

                case "host":
                    // Header restrito, ignorar
                    break;

                case "if-modified-since":
                    if (DateTime.TryParse(value, out var d))
                    {
                        _request.IfModifiedSince = d.ToUniversalTime();
                    }
                    break;

                case "range":
                    // Suporta formatos como bytes=500-999
                    // Tente parse simples, caso não, envie bruto
                    if (value.StartsWith("bytes=", StringComparison.OrdinalIgnoreCase))
                    {
                        var spec = value.Substring(6);
                        var parts = spec.Split('-', 2);
                        if (parts.Length == 2)
                        {
                            if (long.TryParse(parts[0], out var from))
                            {
                                if (long.TryParse(parts[1], out var to))
                                {
                                    _request.AddRange(from, to);
                                    break;
                                }
                                _request.AddRange(from);
                                break;
                            }
                        }
                    }
                    // Falha no parse, deixa cair para headers genéricos
                    goto default;

                case "referer":
                    _request.Referer = value;
                    break;

                case "transfer-encoding":
                    _request.TransferEncoding = value;
                    _request.SendChunked = true;
                    break;

                case "user-agent":
                    _request.UserAgent = value;
                    break;

                default:
                    try
                    {
                        _request.Headers.Remove(name);
                        _request.Headers.Add(name, value);
                    }
                    catch
                    {
                        // Alguns nomes ainda podem ser bloqueados pelo runtime
                        // Nesses casos, simplesmente ignore para manter compatibilidade
                    }
                    break;
            }
        }
    }
}
