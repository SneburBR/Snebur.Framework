//using System.Diagnostics;
//using System.IO;
//using System.Net;
//using System.Net.Http;

//namespace Snebur.Utilidade;

//public abstract class HttpClientProxy
//{
//    public WebHeaderCollection Headers { get; set; } = new WebHeaderCollection();
//    public string? ContentType { get; set; }
//    public int ContentLength { get; set; }
//    public string Method { get; set; } = "GET";
//    public int Timeout { get; set; }

//    public static HttpClientProxy Create(string url)
//    {

//#if DEBUG

//        return new ModernHttpClientProxy(url);
//#else

//        return new HttpWebRequestProxy(url);
//#endif
//    }

//    public abstract Stream GetRequestStream();

//    public abstract BaseHttpWebResponseProxy GetResponse();

//}

//public class ModernHttpClientProxy : HttpClientProxy
//{
//    private readonly HttpClient _request;
//    public ModernHttpClientProxy(string url)
//    {
//        this._request = new HttpClient
//        {
//            BaseAddress = new Uri(url)
//        };
//    }

//    public override BaseHttpWebResponseProxy GetResponse()
//    {
//        throw new NotImplementedException();
//    }

//    public override Stream GetRequestStream()
//    {
//        throw new NotImplementedException();
//    }
//}

//public class HttpWebRequestProxy : HttpClientProxy
//{
//    private readonly HttpWebRequest _request;
//    private HttpWebResponseProxy? _httpWebResponseProxy;
//    public HttpWebRequestProxy(string url)
//    {
//#pragma warning disable SYSLIB0014  
//        this._request = (HttpWebRequest)HttpWebRequest.Create(url);
//        this._request.Proxy = null;
//#pragma warning restore SYSLIB0014  
//    }

//    public override Stream GetRequestStream()
//    {
//        return this._request.GetRequestStream();
//    }

//    public override BaseHttpWebResponseProxy GetResponse()
//    {
//        return this._httpWebResponseProxy ??= new HttpWebResponseProxy((HttpWebResponse)this._request.GetResponse());
//    }
//}

//public abstract class BaseHttpWebResponseProxy : IDisposable
//{
//    public HttpStatusCode StatusCode { get; set; }
//    public bool IsStatusCodeOk
//        => this.StatusCode == HttpStatusCode.OK
//        || this.StatusCode == HttpStatusCode.Created;

//    public abstract Stream GetResponseStream();
//    public abstract long ContentLength { get; }
//    public abstract void Dispose();
//}

//public class HttpWebResponseProxy : BaseHttpWebResponseProxy
//{
//    private readonly HttpWebResponse _response;

//    public override long ContentLength
//        => this._response.ContentLength;

//    public HttpWebResponseProxy(HttpWebResponse response)
//    {
//        this._response = response;
//    }

//    public override Stream GetResponseStream()
//    {
//        return this._response.GetResponseStream();
//    }

//    public override void Dispose()
//    {
//        this._response?.Close();
//    }
//}

//public class HttpResponseMessageProxy : BaseHttpWebResponseProxy
//{
//    private readonly HttpResponseMessage _responseMessage;

//    public override long ContentLength
//        => this._responseMessage.Content.Headers.ContentLength ?? 0;

//    public HttpResponseMessageProxy(HttpResponseMessage responseMessage)
//    {
//        this._responseMessage = responseMessage;
//    }
//    public override Stream GetResponseStream()
//    {
//        return this._responseMessage.Content.ReadAsStreamAsync().Result;
//    }
//    public override void Dispose()
//    {
//        this._responseMessage?.Dispose();
//    }
//}