
#if NET7_0 == false

namespace Snebur.Comunicacao
{
    using Snebur.Utilidade;
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.Remoting.Contexts;
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

            if (CrossDomainUtil.VerificarContratoCrossDomain(aplicacao.Context))
            {
                aplicacao.CompleteRequest();
                return;
            }

            this.AntesProcessarRequisicao(aplicacao.Context);

            var response = aplicacao.Context.Response;

            if (this.IsExecutarServico(aplicacao))
            {
                response.StatusCode = 0;
                try
                {
                    this.ExecutarServico(aplicacao);
                }
                catch (Exception ex)
                {
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    if (DebugUtil.IsAttached)
                    {
                        throw ;
                    }
                    LogUtil.ErroAsync(ex);
                }
                finally
                {
                    if (response.StatusCode == 0)
                    {
                        response.StatusCode = (int)HttpStatusCode.Conflict;
                    }
                    aplicacao.CompleteRequest();
                }
            }
        }

        private bool IsExecutarServico(HttpApplication aplicacao)
        {
            var request = aplicacao.Request;
            var response = aplicacao.Response;

            var caminhoCompleto = request.RetornarUrlRequisicao().LocalPath.ToLower();
            if (this.DiretoriosImagemAutorizado.Any(x => caminhoCompleto.StartsWith(x)))
            {
                var caminhoImagem = aplicacao.Server.MapPath(caminhoCompleto);
                response.ContentType = "image/jpeg";
                response.StatusCode = 200;
                response.WriteFile(caminhoImagem);
                aplicacao.CompleteRequest();

                return false;
            }

            var caminho = Path.GetFileName(caminhoCompleto).ToLower();
            if (caminho == "/" || caminho == "")
            {
                this.ResponderAgora(response);
                aplicacao.CompleteRequest();
                return false;
            }

            if (caminho.Equals("agora", StringComparison.InvariantCultureIgnoreCase))
            {
                this.ResponderAgora(response);
                aplicacao.CompleteRequest();
                return false;
            }

            if (caminho.Equals("ping", StringComparison.InvariantCultureIgnoreCase))
            {
                response.Write("True");
                aplicacao.CompleteRequest();
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
                        aplicacao.CompleteRequest();
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
                    aplicacao.CompleteRequest();
                    return false;
                }

                this.ExecutarManipuladorGenerico(aplicacao.Context, caminhoManipulador);
                aplicacao.CompleteRequest();
                return false;
            }


            var allKeys = request.Headers.AllKeys;
            if (!allKeys.Contains(ParametrosComunicacao.TOKEN, new IgnorarCasoSensivel()) ||
                !allKeys.Contains(ParametrosComunicacao.MANIPULADOR, new IgnorarCasoSensivel()))
            {
                LogUtil.SegurancaAsync(String.Format("A url '{0}' foi chamada incorretamente.", request.Url.AbsoluteUri), Servicos.EnumTipoLogSeguranca.CabecalhoInvalido);

                response.StatusCode = (int)HttpStatusCode.BadRequest;
                aplicacao.CompleteRequest();
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
                if(tipoManipulador== null)
                {
                    this.NotificarServicoNaoEncontado(aplicacao.Context, nomeManipulador);
                    return;
                }
                using (var servico = (BaseComunicacaoServidor)Activator.CreateInstance(tipoManipulador))
                {
                    try
                    {
                        servico.IdentificadorProprietario = identificadorProprietario;
                        servico.ProcessRequest(aplicacao.Context);
                    }
                    catch (ErroRequisicao)
                    {
                        throw ;
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
