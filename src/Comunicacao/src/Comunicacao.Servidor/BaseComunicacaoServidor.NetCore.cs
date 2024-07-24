#if NET6_0_OR_GREATER

namespace Snebur.Comunicacao
{
    using Microsoft.AspNetCore.Http;
    using Snebur.Servicos;
    using Snebur.Utilidade;
    using System;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    public abstract partial class BaseComunicacaoServidor
    {
        public HttpContext HttpContext { get; private set; }

        public async Task ProcessRequestAsync(HttpContext httpContext)
        {
            this.HttpContext = httpContext; 
            var response = httpContext.Response;
            try
            {
                this.AntesProcessRequest(httpContext);

                response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
                response.Headers.Append("Pragma", "no-cache");
                response.Headers.Append("Expires", "0");

                
                using (var requisicao = new Requisicao(httpContext,
                                                       this.CredencialServico,
                                                       this.IdentificadorProprietario,
                                                       this.GetType().Name))
                {
                    await requisicao.ProcessarAsync();

                    if (requisicao.CredencialServicoValida())
                    {
                        this.InformacaoSessao = requisicao.InformacaoSessaoUsuario;
                        this.CredencialUsuario = requisicao.CredencialUsuario;
                        this.IdentificadorSessaoUsuario = requisicao.IdentificadorSessaoUsuario;
                        this.Inicializar(requisicao);

                        try
                        {
                            var resultadoSerializado = this.RetornarResultadoChamadaSerializado(requisicao, httpContext);
                            httpContext.Response.StatusCode = (int)HttpStatusCode.OK;

                            if (!requisicao.ContratoChamada.Async && requisicao.IsSerializarJavascript)
                            {
                                response.ContentType = "text/json; charset=utf-8";
                                await response.WriteAsync(resultadoSerializado);
                            }
                            else
                            {
                                var conteudo = PacoteUtil.CompactarPacote(resultadoSerializado);
                                await response.Body.WriteAsync(conteudo);
                            }
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

                        //response.SubStatusCode = Convert.ToInt32(EnumTipoLogSeguranca.CredencialServicoNaoAutorizada);

                        LogUtil.SegurancaAsync(mensagemSeguranca, EnumTipoLogSeguranca.CredencialServicoNaoAutorizada);
                    }
                }
            }
            catch (ErroDeserializarContrato)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                //response.SubStatusCode = Convert.ToInt32(EnumTipoLogSeguranca.ContratoInvalido);

                var mensagemSeguranca = String.Format("O contrato da chamada é invalido '{0}' '{1}' - '{2}' ", this.GetType().Name, this.CredencialServico.IdentificadorUsuario, this.CredencialServico.Senha);
                LogUtil.SegurancaAsync(mensagemSeguranca, EnumTipoLogSeguranca.ContratoInvalido);
            }
            catch (ErroMetodoOperacaoNaoFoiEncontrado erro)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                //response.SubStatusCode = Convert.ToInt32(EnumTipoLogSeguranca.MetodoOperacaoNaoEncontrado);

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
}
#endif
