using Snebur.Dominio;
using Snebur.Seguranca;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Snebur
{
    public class AplicacaoSneburAspNet : AplicacaoSnebur
    {
        public const string PARAMETRO_IP_REQUISICAO = "IpRequisicao";

        public override EnumTipoAplicacao TipoAplicacao => EnumTipoAplicacao.DotNet_WebService;

        public virtual HttpContext HttpContext
        {
            get
            {
#if NetCore
                return this.FuncaoRetornaHttpContextAtual?.Invoke();
#else

                return HttpContext.Current;
#endif
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override InformacaoSessaoUsuario InformacaoSessaoUsuarioRequisicaoAtual
        {
            get
            {
                var httpContext = this.HttpContext;
                if (httpContext != null)
                {
                    lock (httpContext.Items.SyncRoot)
                    {
                        if (httpContext.Items.Contains(ConstantesItensRequsicao.CHAVE_INFORMACAO_SESSAO_ATUAL))
                        {
                            return (InformacaoSessaoUsuario)httpContext.Items[ConstantesItensRequsicao.CHAVE_INFORMACAO_SESSAO_ATUAL];
                        }
                    }
                }
                return base.InformacaoSessaoUsuarioRequisicaoAtual;
            }
        }
         
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string IdentificadorProprietarioRequisicaoAtual
        {
            get
            {
                var httpContext = this.HttpContext;
                if (httpContext != null)
                {
                    lock (httpContext.Items.SyncRoot)
                    {
                        if (httpContext.Items.Contains(ConstantesItensRequsicao.CHAVE_IDENTIFICADOR_PROPRIETARIO))
                        {
                            return (string)httpContext.Items[ConstantesItensRequsicao.CHAVE_IDENTIFICADOR_PROPRIETARIO];
                        }
                    }
                }
                return base.IdentificadorProprietarioRequisicaoAtual;

            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Guid IdentificadorSessaoUsuarioRequisicaoAtual
        {
            get
            {
                var httpContext = this.HttpContext;
                if (httpContext != null)
                {
                    lock (httpContext.Items.SyncRoot)
                    {
                        if (httpContext.Items.Contains(ConstantesItensRequsicao.CHAVE_INFORMACAO_SESSAO_ATUAL))
                        {
                            return (httpContext.Items[ConstantesItensRequsicao.CHAVE_INFORMACAO_SESSAO_ATUAL] as InformacaoSessaoUsuario).IdentificadorSessaoUsuario;
                        }
                    }
                }
                return base.IdentificadorSessaoUsuarioRequisicaoAtual;

            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override CredencialUsuario CredencialUsuarioRequisicaoAtual
        {
            get
            {
                var httpContext = this.HttpContext;
                if (httpContext != null)
                {
                    lock (httpContext.Items.SyncRoot)
                    {
                        if (httpContext.Items.Contains(ConstantesItensRequsicao.CHAVE_CREDENCIAL_USUARIO))
                        {
                            return (CredencialUsuario)httpContext.Items[ConstantesItensRequsicao.CHAVE_CREDENCIAL_USUARIO];
                        }
                    }
                }
                return base.CredencialUsuarioRequisicaoAtual;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string UserAgent => this.HttpContext?.Request?.UserAgent;

        public override string RetornarIpDaRequisicao()
        {
           
            var httpContext = this.HttpContext;
            string ip = null;
            if (httpContext != null)
            {
#if NetCore
                ip = httpContext.Connection.RemoteIpAddress.ToString();
#else
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
#endif
                if (ip == ConstantesIP.IP6_LOCAL)
                {
                    return ConstantesIP.IP_LOCAL;
                }
                if (ValidacaoUtil.IsIp(ip))
                {
                    return ip;
                }
            }

            return base.RetornarIp();
            //throw new Exception("Não foi possível retornar o IP da requisição");
        }

        public static AplicacaoSneburAspNet AtualAspNet => AplicacaoSnebur.Atual as AplicacaoSneburAspNet;
    }
}
