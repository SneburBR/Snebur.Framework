using Snebur.Comunicacao;
using Snebur.Dominio;
using Snebur.Seguranca;
using Snebur.Utilidade;
using System;
using System.Collections;
using System.ComponentModel;
using System.Web;

#if NET7_0
using Microsoft.AspNetCore.Http;
#else
//using System.Web;
#endif 

namespace Snebur
{
    public class AplicacaoSneburAspNet : AplicacaoSnebur, IAplicacaoSneburAspNet
    {
        public const string PARAMETRO_IP_REQUISICAO = "IpRequisicao";
        public override EnumTipoAplicacao TipoAplicacao => EnumTipoAplicacao.DotNet_WebService;

#if NET7_0  == false
        public virtual HttpContext HttpContext => HttpContext.Current;
#endif

        public string IpRequisicao
        {
            get
            {
                if (this.IsPossuiRequisicaoAspNetAtiva)
                {
                    return this.IP;
                }
                return null;
            }
        }


        [EditorBrowsable(EditorBrowsableState.Never)]
        public override InformacaoSessaoUsuario InformacaoSessaoUsuario
        {
            get
            {
                var httpContext = this.HttpContext;
                if (httpContext != null)
                {
                    lock ((httpContext.Items as ICollection).SyncRoot)
                    {

                        if (httpContext.Items.ContainsKey(ConstantesItensRequsicao.CHAVE_INFORMACAO_SESSAO_ATUAL))
                        {
                            return (InformacaoSessaoUsuario)httpContext.Items[ConstantesItensRequsicao.CHAVE_INFORMACAO_SESSAO_ATUAL];
                        }
                    }
                }
                return base.InformacaoSessaoUsuario;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string IdentificadorProprietario
        {
            get
            {
                var httpContext = this.HttpContext;
                if (httpContext != null)
                {
                    lock ((httpContext.Items as ICollection).SyncRoot)
                    {
                        if (httpContext.Items.ContainsKey(ConstantesItensRequsicao.CHAVE_IDENTIFICADOR_PROPRIETARIO))
                        {
                            return (string)httpContext.Items[ConstantesItensRequsicao.CHAVE_IDENTIFICADOR_PROPRIETARIO];
                        }
                    }
                }
                return base.IdentificadorProprietario;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Guid IdentificadorSessaoUsuario
        {
            get
            {
                var httpContext = this.HttpContext;
                if (httpContext != null)
                {
                    lock ((httpContext.Items as ICollection).SyncRoot)
                    {
                        if (httpContext.Items.ContainsKey(ConstantesItensRequsicao.CHAVE_INFORMACAO_SESSAO_ATUAL))
                        {
                            return (httpContext.Items[ConstantesItensRequsicao.CHAVE_INFORMACAO_SESSAO_ATUAL] as InformacaoSessaoUsuario).IdentificadorSessaoUsuario;
                        }
                    }
                }
                return base.IdentificadorSessaoUsuario;

            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override CredencialUsuario CredencialUsuario
        {
            get
            {
                var httpContext = this.HttpContext;
                if (httpContext != null)
                {
                    lock ((httpContext.Items as ICollection).SyncRoot)
                    {
                        if (httpContext.Items.ContainsKey(ConstantesItensRequsicao.CHAVE_CREDENCIAL_USUARIO))
                        {
                            return (CredencialUsuario)httpContext.Items[ConstantesItensRequsicao.CHAVE_CREDENCIAL_USUARIO];
                        }
                    }
                }
                return base.CredencialUsuario;
            }
        }

#if NET7_0  == false

        public override string IP
        {
            get
            {
                var httpContext = this.HttpContext;
                string ip = null;
                if (httpContext != null)
                {
                    if (httpContext.Request.Headers[PARAMETRO_IP_REQUISICAO] != null)
                    {
                        var ipRequisicao = httpContext.Request.Headers[PARAMETRO_IP_REQUISICAO];
                        if (IpUtil.IsIP(ipRequisicao))
                        {
                            return ipRequisicao;
                        }
                    }
                    ip = httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (!ValidacaoUtil.IsIp(ip))
                    {
                        ip = httpContext.Request.ServerVariables["REMOTE_ADDR"];
                    }
                    if (ip == ConstantesIP.IP6_LOCAL)
                    {
                        return ConstantesIP.IP_LOCAL;
                    }
                    if (ValidacaoUtil.IsIp(ip))
                    {
                        return ip;
                    }
                }
                return base.IP;
            }


            //throw new Exception("Não foi possível retornar o IP da requisição");
        }
#endif

        //public static AplicacaoSneburAspNet AtualAspNet => AplicacaoSnebur.Atual as AplicacaoSneburAspNet;

        #region IAplicacaoSneburAspNet
        public bool IsPossuiRequisicaoAspNetAtiva => this.HttpContext?.Request != null;

        public string RetornarValueCabecalho(string chave)
        {
            throw new NotImplementedException();
        }

#if NET7_0  == false
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string UserAgent => this.HttpContext?.Request?.UserAgent;
#else
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string UserAgent => this.HttpContext.Request.UserAgent();
#endif
        public T GetHttpContext<T>()
        {
            if (this.HttpContext == null)
            {
                return default;
            }

            if (this.HttpContext is T httpContext)
            {
                return httpContext;
            }
            throw new Exception($"Não foi possível converter HttpContext para o tipo {typeof(T).Name}");
        }

        public InfoRequisicao RetornarInfoRequisicao()
        {
            var httpContext = this.HttpContext;
            if (httpContext == null)
            {
                return null;
            }

            return new InfoRequisicao
            {
                CredencialUsuario = this.CredencialUsuario.IdentificadorUsuario,
                IpRequisicao = this.IpRequisicao,
                UserAgent = this.UserAgent
            };
        }

        #endregion

#if NET7_0

        #region IAplicacaoSneburAspNetCore

        //private static IServiceProvider ServiceProvider;
        private static IHttpContextAccessor HttpContextAccessor;

        public virtual HttpContext HttpContext
        {
            get
            {
                if (AplicacaoSneburAspNet.HttpContextAccessor == null)
                {
                    throw new Exception("O HttpContextAccessor não foi definido");
                }
                return AplicacaoSneburAspNet.HttpContextAccessor.HttpContext;
            }
        }

        public void ConfigureHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            AplicacaoSneburAspNet.HttpContextAccessor = httpContextAccessor;
            //AplicacaoSneburAspNet.ServiceProvider = ServiceProvider;
        }

        #endregion
#endif
    }
}
