
#if NetCore == false
 
namespace Snebur.Comunicacao
{
    using Snebur.Utilidade;
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;

    public abstract partial class BaseManipuladorRequisicao : IHttpModule
    {
        public void Init(HttpApplication aplicacao)
        {
            aplicacao.BeginRequest += (new EventHandler(this.Aplicacao_BeginRequest));
        }

        private void Aplicacao_BeginRequest(object sender, EventArgs e)
        {
            var aplicacao = (HttpApplication)sender;
            this.AntesProcessarRequisicao(aplicacao.Context);

            var request = aplicacao.Request;
            var response = aplicacao.Context.Response;

            if (this.IsExecutarServico(aplicacao))
            {
                response.StatusCode = 0;
                try
                {
                    var allKeys = request.Headers.AllKeys;
                    if (allKeys.Contains(ParametrosComunicacao.TOKEN, new IgnorarCasoSensivel()) &&
                        allKeys.Contains(ParametrosComunicacao.MANIPULADOR, new IgnorarCasoSensivel()))
                    {
                        this.ExecutarServico(aplicacao);
                    }
                    else
                    {
                        LogUtil.SegurancaAsync(String.Format("A url '{0}' foi chamada incorretamente.", request.Url.AbsoluteUri), Servicos.EnumTipoLogSeguranca.CabecalhoInvalido);
                    }
                    //Chamadas do cross domain do ajax serão implementas aqui
                }
                catch (Exception ex)
                {
                    var host = aplicacao.Request.Url.Host;
                    if (host.EndsWith(".local") || host.EndsWith("interno"))
                    {
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    }
                    else
                    {
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    }

                    if (DebugUtil.IsAttached)
                    {
                        throw ex;
                    }
                    LogUtil.ErroAsync(ex);
                }
                finally
                {
                    if (response.StatusCode == 0)
                    {
                        response.StatusCode = (int)HttpStatusCode.Conflict;
                        response.SubStatusCode = -1;
                    }
                    aplicacao.CompleteRequest();
                }
            }
        }

        private bool IsExecutarServico(HttpApplication context)


        {
            var request = context.Request;
            var response = context.Response;

            var caminho = Path.GetFileName(request.RetornarUrlRequisicao().LocalPath).ToLower();

            if (CrossDomainUtil.VerificarContratoCrossDomain(context.Context))
            {
                context.CompleteRequest();
                return false;
            }

            if (caminho.Equals("agora", StringComparison.InvariantCultureIgnoreCase))
            {
                this.ResponderAgora(response);

                context.CompleteRequest();
                return false;
            }

            if (caminho.Equals("ping", StringComparison.InvariantCultureIgnoreCase))
            {
                response.Write("True");
                context.CompleteRequest();
                return false;
            }

            if (this.ArquivosAutorizados.ContainsKey(caminho))
            {
                var isIgnorarValidacaoTokenAplicacao = this.ArquivosAutorizados[caminho];
                if (!isIgnorarValidacaoTokenAplicacao)
                {
                    if (!this.IsRequicaoValida(request, false))
                    {
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        context.CompleteRequest();
                        return false;
                    }
                }
                return false;
            }

            var caminhoManipulador = Path.GetFileNameWithoutExtension(caminho);
            if (this.ManipuladoresGenericos.ContainsKey(caminhoManipulador))
            {
                var isValidarToken = this.ManipuladoresGenericos[caminhoManipulador].isValidarToken;
                if (isValidarToken && !this.IsRequicaoValida(request, false))
                {

                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.CompleteRequest();
                    return false;
                }

                this.ExecutarManipuladorGenerico(context.Context, caminhoManipulador);
                context.CompleteRequest();
                return false;
            }


            return true;

        }

        private void ExecutarManipuladorGenerico(HttpContext httpContext, string caminho)
        {

            var tipo = this.ManipuladoresGenericos[caminho].tipo;
            var manipualador = Activator.CreateInstance(tipo) as IHttpHandler;
            if (manipualador != null)
            {
                manipualador.ProcessRequest(httpContext);
            }
        }

        private void ExecutarServico(HttpApplication aplicacao)
        {
            var request = aplicacao.Request;

            if (this.IsRequicaoValida(request, true))
            {
                var identificadorProprietario = this.RetornarIdentificadorProprietario(request);
                var nomeManipulador = request.Headers[ParametrosComunicacao.MANIPULADOR];
                var tipoManipulador = this.RetornarTipoServico(nomeManipulador);
                using (var servico = (BaseComunicacaoServidor)Activator.CreateInstance(tipoManipulador))
                {
                    try
                    {
                        servico.IdentificadorProprietario = identificadorProprietario;
                        servico.ProcessRequest(aplicacao.Context);
                    }
                    catch (ErroRequisicao ex)
                    {
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        throw new ErroWebService(ex, nomeManipulador);
                    }
                }
            }
        }

        private void ResponderAgora(HttpResponse response)
        {
            var agora = DateTime.UtcNow.AddSeconds(-10);
            response.ContentEncoding = Encoding.UTF8;
            response.ContentType = "text/text";
            response.Charset = "utf8";
            response.StatusCode = 200;
            response.Write(agora.Ticks);
        }

    }
}

#endif
