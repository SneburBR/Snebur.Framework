using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;

namespace Snebur
{
    public class AplicacaoSneburAspNet : AplicacaoSnebur
    {
        private static IServiceProvider ServiceProvider;


        private static IHttpContextAccessor HttpContextAccessor;

        public virtual HttpContext HttpContext
        {
            get
            {
                return AplicacaoSneburAspNet.HttpContextAccessor.HttpContext;
            }
        }
        public AplicacaoSneburAspNet()
        {

        }

        public void ConfigureHttpContextAccessor(IHttpContextAccessor httpContextAccessor,
                                                 IServiceProvider ServiceProvider)
        {
            AplicacaoSneburAspNet.HttpContextAccessor = httpContextAccessor;
            AplicacaoSneburAspNet.ServiceProvider = ServiceProvider;
        }

     

        public override string RetornarIpDaRequisicao()
        {
            return this.HttpContext?.Connection.RemoteIpAddress.ToString();
        }
    }
}
