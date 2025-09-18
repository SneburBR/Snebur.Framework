using System.IO;
using System.Net;

namespace Snebur.Net;

public abstract class BaseHttpWebResponseProxy : IDisposable
{
    public HttpStatusCode StatusCode { get; set; }
    public bool IsStatusCodeOk
        => this.StatusCode == HttpStatusCode.OK
           || this.StatusCode == HttpStatusCode.Created;

    public abstract Stream GetResponseStream();
    public abstract long ContentLength { get; }
    public abstract void Dispose();
}
