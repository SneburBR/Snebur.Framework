
namespace Snebur.Comunicacao
{
#if NET7_0 == false

    using Snebur.Servicos;
    using Snebur.Utilidade;
    using System;
    using System.Net;
    using System.Text;
    using System.Web;

    public abstract partial class BaseComunicacaoServidor
    {
        public HttpContext HttpContext { get; private set; }
        public void ProcessRequest(HttpContext httpContext)
        {
            this.HttpContext = httpContext; 
            var response = httpContext.Response;
            try
            {
                this.AntesProcessRequest(httpContext);

                //Sem cache
                response.CacheControl = "no-cache";
                response.AddHeader("Pragma", "no-cache");

                response.Cache.SetNoStore();
                response.Cache.SetCacheability(HttpCacheability.NoCache);
                response.Cache.SetExpires(DateTime.Now.AddYears(-1));

                using (var requisicao = new Requisicao(httpContext,
                                                       this.CredencialServico,
                                                       this.IdentificadorProprietario,
                                                       this.GetType().Name))
                {
                    if (requisicao.CredencialServicoValida())
                    {
                        this.InformacaoSessaoUsuario = requisicao.InformacaoSessaoUsuario;
                        this.CredencialUsuario = requisicao.CredencialUsuario;
                        this.IdentificadorSessaoUsuario = requisicao.InformacaoSessaoUsuario.IdentificadorSessaoUsuario;
                        this.Inicializar(requisicao);

                        try
                        {
                            var resultadoSerializado = this.RetornarResultadoChamadaSerializado(requisicao, httpContext);

                            if (!requisicao.ContratoChamada.Async && requisicao.IsSerializarJavascript)
                            {
                                httpContext.Response.ContentType = "text/json";
                                httpContext.Response.Charset = "utf8";
                                httpContext.Response.ContentEncoding = Encoding.UTF8;
                                httpContext.Response.Write(resultadoSerializado);
                            }
                            else
                            {
                                var conteudo = PacoteUtil.CompactarPacote(resultadoSerializado);
                                httpContext.Response.BinaryWrite(conteudo);

                            }
                            httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                        }

                        catch (Exception ex)
                        {
                            throw new ErroRequisicao(ex, requisicao);
                        }
                    }
                    else
                    {
                        var mensagemSeguranca = String.Format("A credencial do serviço não autorizada '{0}' '{1}' - '{2}' ", this.GetType().Name, this.CredencialServico.IdentificadorUsuario, this.CredencialServico.Senha);
                        if (DebugUtil.IsAttached)
                        {
                            throw new Exception(mensagemSeguranca);
                        }

                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        response.SubStatusCode = Convert.ToInt32(EnumTipoLogSeguranca.CredencialServicoNaoAutorizada);

                        LogUtil.SegurancaAsync(mensagemSeguranca, EnumTipoLogSeguranca.CredencialServicoNaoAutorizada);
                    }
                }
            }
            catch (ErroDeserializarContrato)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.SubStatusCode = Convert.ToInt32(EnumTipoLogSeguranca.ContratoInvalido);

                var mensagemSeguranca = String.Format("O contrato da chamada é invalido '{0}' '{1}' - '{2}' ", this.GetType().Name, this.CredencialServico.IdentificadorUsuario, this.CredencialServico.Senha);
                LogUtil.SegurancaAsync(mensagemSeguranca, EnumTipoLogSeguranca.ContratoInvalido);
            }
            catch (ErroMetodoOperacaoNaoFoiEncontrado erro)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.SubStatusCode = Convert.ToInt32(EnumTipoLogSeguranca.MetodoOperacaoNaoEncontrado);

                var mensagemSeguranca = String.Format("O método '{0}' não foi encontrado no serviço '{1}'", erro.NomeMetodo, this.GetType().Name);
                LogUtil.SegurancaAsync(mensagemSeguranca, EnumTipoLogSeguranca.MetodoOperacaoNaoEncontrado);
            }
            catch (ErroRequisicao)
            {
                throw;
            }
            catch (Exception )
            {
                throw;
            }
        }

    }

#endif
}
