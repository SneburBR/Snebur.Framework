using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Snebur.Comunicacao;
using Snebur.Linq;
using Snebur.Seguranca;
using System.ComponentModel;

namespace Snebur;

public class AplicacaoSneburAspNet : AplicacaoSnebur, IAplicacaoSneburAspNet
{
    public const string PARAMETRO_IP_REQUISICAO = "IpRequisicao";
    public override EnumTipoAplicacao TipoAplicacao => EnumTipoAplicacao.DotNet_WebService;

    public string? IpRequisicao
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
            if (httpContext is not null)
            {
                lock (httpContext.Items.SyncLock())
                {
                    if (httpContext.Items.ContainsKey(ConstantesItensRequsicao.CHAVE_INFORMACAO_SESSAO_ATUAL) &&
                        httpContext.Items[ConstantesItensRequsicao.CHAVE_INFORMACAO_SESSAO_ATUAL] is InformacaoSessao informacao)
                    {
                        return informacao;
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
                    if (httpContext.Items.ContainsKey(ConstantesItensRequsicao.CHAVE_IDENTIFICADOR_PROPRIETARIO) &&
                        httpContext.Items[ConstantesItensRequsicao.CHAVE_IDENTIFICADOR_PROPRIETARIO] is string identificador)
                    {
                        return identificador;
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
                    if (httpContext.Items.ContainsKey(ConstantesItensRequsicao.CHAVE_CREDENCIAL_USUARIO) &&
                       httpContext.Items[ConstantesItensRequsicao.CHAVE_CREDENCIAL_USUARIO] is CredencialUsuario credencialUsuario)
                    {
                        return credencialUsuario;
                    }
                }
            }
            return base.CredencialUsuario;
        }
    }

    //public static AplicacaoSneburAspNet AtualAspNet => AplicacaoSnebur.Atual as AplicacaoSneburAspNet;

    #region IAplicacaoSneburAspNet
    public bool IsPossuiRequisicaoAspNetAtiva => this.HttpContext?.Request != null;

    public string RetornarValueCabecalho(string chave)
    {
        throw new NotImplementedException();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public string? UserAgent => this.HttpContext?.Request?.UserAgent();

    public T? GetHttpContext<T>()
    {
        if (this.HttpContext is null)
        {
            return default;
        }

        if (this.HttpContext is T httpContext)
        {
            return httpContext;
        }
        throw new Exception($"Não foi possível converter HttpContext para o tipo {typeof(T).Name}");
    }

    public InfoRequisicao? RetornarInfoRequisicao()
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
    #region IAplicacaoSneburAspNetCore

    private static IHttpContextAccessor? _httpContextAccessor;

    public virtual HttpContext? HttpContext
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

    public virtual void AddServices(
        IHostApplicationBuilder app,
        IWebHostBuilder webHost,
        IConfigurationRoot configuracao)
    {

    }

    #endregion
}