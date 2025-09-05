#if NET6_0_OR_GREATER

using Microsoft.AspNetCore.Http;

namespace Snebur.AcessoDados.Utilidade;

//https://stackoverflow.com/questions/38571032/how-to-get-httpcontext-current-in-asp-net-core
public static class HttpContextUtil
{

    public static IServiceProvider? ServiceProvider;

    public static HttpContext? Current
    {
        get
        {
            var factory = ServiceProvider?.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
            return factory?.HttpContext;
        }
    }
}

#endif
