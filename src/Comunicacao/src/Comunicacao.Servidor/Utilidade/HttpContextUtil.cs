#if NET6_0_OR_GREATER

using System;

namespace Snebur.AcessoDados.Utilidade
{
    //https://stackoverflow.com/questions/38571032/how-to-get-httpcontext-current-in-asp-net-core
    public static class HttpContextUtil
    {

        public static IServiceProvider ServiceProvider;

        public static Microsoft.AspNetCore.Http.HttpContext Current
        {
            get
            {
                // var factory2 = ServiceProvider.GetService<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
                object factory = ServiceProvider.GetService(typeof(Microsoft.AspNetCore.Http.IHttpContextAccessor));

                // Microsoft.AspNetCore.Http.HttpContextAccessor fac =(Microsoft.AspNetCore.Http.HttpContextAccessor)factory;
                Microsoft.AspNetCore.Http.HttpContext context = ((Microsoft.AspNetCore.Http.HttpContextAccessor)factory).HttpContext;
                // context.Response.WriteAsync("Test");

                return context;
            }
        }



    }
}

#endif
