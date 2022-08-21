using Microsoft.AspNetCore.Http;

namespace Snebur.Net
{
    public class ZyonHttpContextCore : ZyonHttpContext
    {
        public ZyonHttpContextCore(HttpContext httpContext) :
            base(httpContext, new ZyonRequestCore(httpContext.Request), new ZyonResponseCore(httpContext.Response))
        {

        }
    }
}
