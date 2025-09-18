using System.IO;
using System.Net;

namespace Snebur.Net;

public class HttpWebResponseProxy : BaseHttpWebResponseProxy
{
    private readonly HttpWebResponse _response;

    public override long ContentLength => this._response.ContentLength;

    public HttpWebResponseProxy(HttpWebResponse response)
    {
        this._response = response;
        this.StatusCode = response.StatusCode;
    }

    public override Stream GetResponseStream()
    {
        return this._response.GetResponseStream();
    }

    public override void Dispose()
    {
        this._response?.Close();
    }
}
