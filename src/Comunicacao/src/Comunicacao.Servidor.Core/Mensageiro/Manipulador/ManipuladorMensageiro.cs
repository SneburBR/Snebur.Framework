using Microsoft.Web.WebSockets;
using System;
using System.Web;
using Snebur.Seguranca;

namespace Snebur.Comunicacao.Mensageiro
{
    public abstract class ManipuladorMensageiro<TConexaoMensageiro, TCentral> : IHttpHandler where TConexaoMensageiro : ConexaoMensageiro
                                                                                             where TCentral : Central<TConexaoMensageiro>
    {
        private const int TEMPO_EXPIRAR_TOKEN = Token.TEMPO_EXPIRAR_TOKEN_PADRAO;

        public void ProcessRequest(HttpContext context)
        {
            if (context.IsWebSocketRequest)
            {
                var isNovaConexaoInicializada = this.IniciarNovaConexao(context);
                if (!isNovaConexaoInicializada)
                {
                    context.Response.StatusCode = 404;
                }
            }
        }

        private bool IniciarNovaConexao(HttpContext context)
        {
            var (isTokenValidom, identificadorSessaoUsuario, identificadorUnicoConexao, argumentoOpcional) = this.RetornarIdentificadoesToken(context);
            if (isTokenValidom)
            {
                var novaConexao = this.RetornarNovaConexaoMensageiro(identificadorSessaoUsuario, identificadorUnicoConexao, argumentoOpcional);
                if (novaConexao != null)
                {
                    context.AcceptWebSocketRequest(novaConexao);
                    return true;
                }
            }
            return false;
        }

        protected virtual (bool isTokenValido, Guid identificadorSessaoUsuario, Guid identificadorUnicoSessao, object argumentoOpcional) RetornarIdentificadoesToken(HttpContext context)
        {
            var token = context.Request.Path;
            var resultadoToken = Token.ValidarToken<Guid, Guid>(token, TimeSpan.FromSeconds(TEMPO_EXPIRAR_TOKEN));
            var isTokenValido = (resultadoToken.Estado == EnumEstadoToken.Valido);
            var identificadorSessaoUsuario = resultadoToken.Itens.Item1;
            var identificadorUnicoConexao = resultadoToken.Itens.Item2;
            return (isTokenValido, identificadorSessaoUsuario, identificadorUnicoConexao, null);
        }


        protected abstract TConexaoMensageiro RetornarNovaConexaoMensageiro(Guid IdentificadorSessaoUsuario, Guid identificadorUnicoConexao, object argumentoOpcional);

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}