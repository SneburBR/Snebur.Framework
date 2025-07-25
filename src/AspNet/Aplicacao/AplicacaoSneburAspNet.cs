﻿using Snebur.Comunicacao;
using Snebur.Dominio;
using Snebur.Seguranca;

using System;
using System.ComponentModel;
using Snebur.Linq;
  
#if NET6_0_OR_GREATER

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

#else
using Snebur.Utilidade;
using System.Web;
#endif 
#if NET8_0_OR_GREATER

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
#endif
namespace Snebur
{
    public class AplicacaoSneburAspNet : AplicacaoSnebur, IAplicacaoSneburAspNet
    {
        public const string PARAMETRO_IP_REQUISICAO = "IpRequisicao";
        public override EnumTipoAplicacao TipoAplicacao => EnumTipoAplicacao.DotNet_WebService;

#if NET6_0_OR_GREATER  == false
        public virtual HttpContext HttpContext => HttpContext.Current;
#endif

        public string IpRequisicao
        {
            get
            {
                if (this.IsPossuiRequisicaoAspNetAtiva)
                {
                    return this.IpPublico;
                }
                return null;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override InformacaoSessao InformacaoSessao
        {
            get
            {
                var httpContext = this.HttpContext;
                if (httpContext != null)
                {
                    lock (httpContext.Items.SyncLock())
                    {

                        if (httpContext.Items.ContainsKey(ConstantesItensRequsicao.CHAVE_INFORMACAO_SESSAO_ATUAL))
                        {
                            return (InformacaoSessao)httpContext.Items[ConstantesItensRequsicao.CHAVE_INFORMACAO_SESSAO_ATUAL];
                        }
                    }
                }
                return base.InformacaoSessao;
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
                    lock (httpContext.Items.SyncLock())
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
                    lock (httpContext.Items.SyncLock())
                    {
                        if (httpContext.Items.ContainsKey(ConstantesItensRequsicao.CHAVE_IDENTIFICADOR_SESSAO_ATUAL))
                        {
                            var identificadorSessaoUsuarioString = httpContext.Items[ConstantesItensRequsicao.CHAVE_IDENTIFICADOR_SESSAO_ATUAL]?.ToString();
                            if (!String.IsNullOrEmpty(identificadorSessaoUsuarioString))
                            {
                                if (Guid.TryParse(identificadorSessaoUsuarioString,
                                            out var identificadorSessaoUsuario))
                                {
                                    return identificadorSessaoUsuario;
                                }
                                throw new Exception($" Não foi possível converter o valor {identificadorSessaoUsuarioString} para Guid");
                            }
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
                    lock (httpContext.Items.SyncLock())
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

#if NET6_0_OR_GREATER  == false

        public override string IpPublico
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
                return base.IpPublico;
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

#if NET6_0_OR_GREATER  == false
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string UserAgent => this.HttpContext?.Request?.UserAgent;
#else
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string UserAgent => this.HttpContext?.Request?.UserAgent();
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

#if NET6_0_OR_GREATER

        #region IAplicacaoSneburAspNetCore

        private static IHttpContextAccessor _httpContextAccessor;

        public virtual HttpContext HttpContext
            => this.HttpContextAccessor.HttpContext;
        public virtual IHttpContextAccessor HttpContextAccessor
            => AplicacaoSneburAspNet._httpContextAccessor ?? throw new Exception("O HttpContextAccessor não foi definido");

        public void ConfigureHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            AplicacaoSneburAspNet._httpContextAccessor = httpContextAccessor;
        }

        public virtual void Configure(WebApplication app,
                                     IWebHostEnvironment env)
        {
        }
 
        #endregion
#endif

#if NET8_0_OR_GREATER
        public virtual void AddServices(
            IHostApplicationBuilder app,
            IWebHostBuilder webHost,
            IConfigurationRoot configuracao)
        {
           
        }
#endif
    }
}
