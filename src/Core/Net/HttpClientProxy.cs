using System.IO;
using System.Net;

namespace Snebur.Net;

public abstract class HttpClientProxy
{
    public WebHeaderCollection Headers { get; set; } = new WebHeaderCollection();
    public string? ContentType { get; set; }
    public int ContentLength { get; set; }
    public string Method { get; set; } = "GET";
    public int Timeout { get; set; }

    public static HttpClientProxy Create(string url)
    {
#if DEBUG
        return new ModernHttpClientProxy(url);
#else
        return new HttpWebRequestProxy(url);
#endif
    }

    public abstract Stream GetRequestStream();
    public abstract BaseHttpWebResponseProxy GetResponse();
}
