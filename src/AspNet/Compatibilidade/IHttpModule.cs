
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace System.Web;

public interface IHttpModule
{
    IConfigurationRoot Configuration { get; }

    void ConfigureServices(IServiceCollection services);

    //void Configure(IApplicationBuilder app, 
    //               IWebHostEnvironment env, 
    //               ILoggerFactory loggerFactory, 
    //               IServiceProvider svp);

}
