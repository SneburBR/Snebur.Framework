
namespace Snebur.Comunicacao
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Text;
    using System.Web;

    public abstract partial class BaseManipuladorRequisicao : IHttpModule
    {
        public IConfigurationRoot Configuration => throw new NotImplementedException();

        public void ConfigureServices(IServiceCollection services)
        {
            throw new NotImplementedException();
        }

        public void Configure(IApplicationBuilder app, 
                              IWebHostEnvironment env, 
                              ILoggerFactory loggerFactory, 
                              IServiceProvider svp)
        {
            throw new NotImplementedException();
        }

        //private void ResponderAgora(HttpResponse response)
        //{
        //    var agora = DateTime.UtcNow.AddSeconds(-10);
        //    response.ContentEncoding = Encoding.UTF8;
        //    response.ContentType = "text/text";
        //    response.Charset = "utf8";
        //    response.StatusCode = 200;
        //    response.Write(agora.Ticks);
        //}
    }
}
