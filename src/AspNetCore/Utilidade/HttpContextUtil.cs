using Microsoft.AspNetCore.Http;
using System;

namespace Snebur.Utilidade
{
    //https://stackoverflow.com/questions/38571032/how-to-get-httpcontext-current-in-asp-net-core
    public static class HttpContextUtil
    {

        public static IServiceProvider ServiceProvider;

        public static HttpContext Current
        {
            get
            {
                
                // var factory2 = ServiceProvider.GetService<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
                object factory = ServiceProvider.GetService(typeof(IHttpContextAccessor));

                // Microsoft.AspNetCore.Http.HttpContextAccessor fac =(Microsoft.AspNetCore.Http.HttpContextAccessor)factory;
                HttpContext context = ((HttpContextAccessor)factory).HttpContext;
                // context.Response.WriteAsync("Test");

                return context;
            }
        }



    }
}
