using Microsoft.AspNetCore.Http;
using Snebur.AspNetCore;
using Snebur.Dominio;
using System;

namespace Snebur
{
    public class AplicacaoSneburAspNet : AplicacaoSnebur, IAplicacaoSneburAspNetCore, IAplicacaoSneburAspNet
    {
        public override EnumTipoAplicacao TipoAplicacao => EnumTipoAplicacao.DotNet_WebService;
        public AplicacaoSneburAspNet()
        {

        }

        protected override string RetornarIp()
        {
            return this.HttpContext?.Connection.RemoteIpAddress.ToString();
        }

        #region IAplicacaoSneburAspNetCore

        private static IServiceProvider ServiceProvider;
        private static IHttpContextAccessor HttpContextAccessor;

        public virtual HttpContext HttpContext
        {
            get
            {
                if(AplicacaoSneburAspNet.HttpContextAccessor == null)
                {
                    throw new Exception("O HttpContextAccessor não foi definido");
                }
                return AplicacaoSneburAspNet.HttpContextAccessor.HttpContext;
            }
        }
         
        public void ConfigureHttpContextAccessor(IHttpContextAccessor httpContextAccessor,
                                                 IServiceProvider ServiceProvider)
        {
            AplicacaoSneburAspNet.HttpContextAccessor = httpContextAccessor;
            AplicacaoSneburAspNet.ServiceProvider = ServiceProvider;
        }

        #endregion


        #region IAplicacaoSneburAspNet
        public bool IsPossuiRequisicaoAspNetAtiva => this.HttpContext != null;

        public string RetornarValueCabecalho(string chave)
        {
            if (this.HttpContext?.Request.Headers.ContainsKey(chave) == true)
            {
                return this.HttpContext?.Request.Headers.GetValue(chave);
            }
            return null;
        }

        public T GetHttpContext<T>()
        {
            if(this.HttpContext == null)
            {
                return default;
            }
            if(this.HttpContext is T context)
            {
                return context;
            }
            throw new Exception($"Não foi possível converter o HttpContext para o tipo {typeof(T).Name} ");
        }

        public string UserAgent => this.HttpContext.Request.UserAgent();

        #endregion
    }
}
