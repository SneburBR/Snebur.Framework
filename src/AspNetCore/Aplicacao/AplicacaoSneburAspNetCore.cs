using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Specialized;
using System.Linq;

namespace Snebur
{
    public class AplicacaoSneburAspNetCore : AplicacaoSnebur
    {
        private static IServiceProvider ServiceProvider;


        private static IHttpContextAccessor HttpContextAccessor;
        public static IConfigurationRoot Configuration { get; set; }

        public AplicacaoSneburAspNetCore()
        {
            this.FuncaoRetornaHttpContextAtual = this.RetornarHttpContextAtual;
        }

        public void ConfigureHttpContextAccessor(IHttpContextAccessor httpContextAccessor, 
                                                 IServiceProvider ServiceProvider)
        {
            AplicacaoSneburAspNetCore.HttpContextAccessor = httpContextAccessor;
            AplicacaoSneburAspNetCore.ServiceProvider = ServiceProvider;
        }


        public void Configure(IConfigurationRoot configuration)
        {
            AplicacaoSneburAspNetCore.Configuration = configuration;
        }

        private HttpContext RetornarHttpContextAtual()
        {
            return HttpContextAccessor.HttpContext;
        }

        protected override NameValueCollection RetornarAppSettings()
        {
            const string CAHVE_APP_CONFICURATION = "AppConfiguration:";
            return this.RetornarConfiguracoes(CAHVE_APP_CONFICURATION);
        }

        protected override NameValueCollection RetornarConnectionStrings()
        {
            const string CAHVE_CONNECTION_STRINGS = "ConnectionStrings:";
            return this.RetornarConfiguracoes(CAHVE_CONNECTION_STRINGS);
        }

        private NameValueCollection RetornarConfiguracoes(string chaveConfiguracao)
        {
            if (AplicacaoSneburAspNetCore.Configuration == null)
            {
                throw new Erro("A propriente Configuration nao foi definida");
            }

            var colecao = new NameValueCollection();

            var paresChaveValor = AplicacaoSneburAspNetCore.Configuration.AsEnumerable().ToList();
            foreach (var parChaveValor in paresChaveValor)
            {
                var chave = parChaveValor.Key;
                if (chave.StartsWith(chaveConfiguracao))
                {
                    var valor = parChaveValor.Value;
                    var chaveNormalizada = chave.Substring(chaveConfiguracao.Length);
                    colecao.Add(chaveNormalizada, valor);
                }
            }
            return colecao;
        }

    }
}
