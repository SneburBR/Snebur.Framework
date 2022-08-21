using System;
using System.Net;


namespace Snebur.Net
{
    public partial class SnHttpContext : IDisposable
    {
        public SnRequest Request { get; }

        public SnResponse Response { get; }

        public object ContextoNativo { get; }


#if NetCore == false

        public SnHttpContext(System.Web.HttpContext httpContext)
        {
            this.ContextoNativo = httpContext;
            this.Request = new SnRequestHttp(httpContext.Request);
            this.Response = new SnResponseHttp(httpContext.Response);
            AdicioanrContexto(this);
        }
#endif

        public SnHttpContext(object contextoNativo, SnRequest request, SnResponse response)
        {
            this.ContextoNativo = contextoNativo;
            this.Request = request;
            this.Response = response;
            AdicioanrContexto(this);
        }

        public SnHttpContext(HttpListenerContext httpContext)
        {
            this.ContextoNativo = httpContext;
            this.Request = new SnRequestHttpListener(httpContext.Request);
            this.Response = new SnResponseHttpListener(httpContext.Response);
            AdicioanrContexto(this);
        }

        public void Dispose()
        {
            RemoverContexto(this);
        }
    }
}
