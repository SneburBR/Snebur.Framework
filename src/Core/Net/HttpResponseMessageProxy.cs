using System.IO;
using System.Net.Http;

namespace Snebur.Net;

public class HttpResponseMessageProxy : BaseHttpWebResponseProxy
{
    private readonly HttpResponseMessage _responseMessage;

    public override long ContentLength
        => this._responseMessage.Content.Headers.ContentLength ?? 0;

    public HttpResponseMessageProxy(HttpResponseMessage responseMessage)
    {
        this._responseMessage = responseMessage;
        this.StatusCode = responseMessage.StatusCode;
    }

    public override Stream GetResponseStream()
    {
        return this._responseMessage.Content.ReadAsStreamAsync().Result;
    }

    public override void Dispose()
    {
        this._responseMessage?.Dispose();
    }
}
