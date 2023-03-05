using System;
using System.Net;
using System.Net.Http;

namespace Snebur.Net
{
    public partial class SnHttpContext : IDisposable
    {
        public SnRequest Request { get; }

        public SnResponse Response { get; }

        public HttpContent ContextoNativo { get; }
         
        public SnHttpContext(HttpContent contextoNativo, 
                            SnRequest request, SnResponse response)
        {
            this.ContextoNativo = contextoNativo;
            this.Request = request;
            this.Response = response;
            //AdicioanrContexto(this);
        }

        public SnHttpContext(HttpListenerContext httpContext)
        {
            this.ContextoNativo = httpContext;
            this.Request = new SnRequestHttpListener(httpContext.Request);
            this.Response = new SnResponseHttpListener(httpContext.Response);
            //AdicioanrContexto(this);
        }

        public void Dispose()
        {
            //RemoverContexto(this);
        }
    }
}
