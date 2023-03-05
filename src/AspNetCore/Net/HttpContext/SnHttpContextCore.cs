using Microsoft.AspNetCore.Http;
using Snebur.Net;

namespace Snebur.AspNetCore.Net.HttpContext
{
    public class SnHttpContextCore : SnHttpContext
    {
        public SnHttpContextCore(HttpContext httpContext) :
            base(httpContext, new ZyonRequestCore(httpContext.Request), new SnResponseCore(httpContext.Response))
        {

        }
    }
}
