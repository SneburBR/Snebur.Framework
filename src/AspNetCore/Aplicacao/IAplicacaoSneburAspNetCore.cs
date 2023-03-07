using Microsoft.AspNetCore.Http;
using System;

namespace Snebur
{
    public interface IAplicacaoSneburAspNetCore
    {
        HttpContext HttpContext { get; }
        void ConfigureHttpContextAccessor(IHttpContextAccessor httpContextAccessor,
                                                IServiceProvider ServiceProvider);

    }
}
