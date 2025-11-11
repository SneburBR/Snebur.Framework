namespace Snebur.Comunicacao;

using Microsoft.AspNetCore.Http;
using Snebur.Servicos;
using Snebur.Utilidade;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

public abstract partial class BaseComunicacaoServidor
{
    public ComunicaoRequisicaoInfo? Info { get; private set; }
    public HttpContext? HttpContext { get; private set; }

    public async Task ProcessRequestAsync(HttpContext httpContext)
    {
        var sw = Stopwatch.StartNew();
        this.HttpContext = httpContext;
        var response = httpContext.Response;
        try
        {
            this.AntesProcessRequest(httpContext);

            response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
            response.Headers.Append("Pragma", "no-cache");
            response.Headers.Append("Expires", "0");

            if (httpContext.Request.Headers.TryGetValue("User-Agent", out var userAgent))
            {
                this.UserAgent = userAgent.ToString();
            }

            using (var requisicao = new Requisicao(httpContext,
                                                   this.CredencialServico,
                                                   this.IdentificadorProprietario,
                                                   this.GetType().Name))
            {
                await requisicao.ProcessarRequisicaoAsync();

                this.Info = requisicao.CreateRequisicaoInfo();
                if (requisicao.IsRequsicaoValida)
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
                        throw new ErroRequisicao(ex, this.Info);
                    }
                }
                else
                {
                    var mensagemSeguranca =
                        $"Contrato invalido ou credencial do serviço não autorizada '{this.GetType().Name}' '{this.CredencialServico.IdentificadorUsuario} <> {requisicao.CredencialServico?.IdentificadorUsuario ?? "null"}' - '{this.CredencialServico.Senha} <> {requisicao.CredencialServico?.Senha ?? "null"}'";
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
        catch (ErroDeserializarContrato ex)
        {
            LogError("BaseComunicacaoServidor.ProcessRequestAsync", ex);
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            //response.SubStatusCode = Convert.ToInt32(EnumTipoLogSeguranca.ContratoInvalido);

            var mensagemSeguranca = String.Format("O contrato da chamada é invalido '{0}' '{1}' - '{2}' ", this.GetType().Name, this.CredencialServico.IdentificadorUsuario, this.CredencialServico.Senha);
            LogUtil.SegurancaAsync(mensagemSeguranca, EnumTipoLogSeguranca.ContratoInvalido);
        }
        catch (ErroMetodoOperacaoNaoFoiEncontrado erro)
        {
            LogError("BaseComunicacaoServidor.ProcessRequestAsync", erro);
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            //response.SubStatusCode = Convert.ToInt32(EnumTipoLogSeguranca.MetodoOperacaoNaoEncontrado);

            var mensagemSeguranca = String.Format("O método '{0}' não foi encontrado no serviço '{1}'", erro.NomeMetodo, this.GetType().Name);
            LogUtil.SegurancaAsync(mensagemSeguranca, EnumTipoLogSeguranca.MetodoOperacaoNaoEncontrado);
        }
        catch (ErroRequisicao ex)
        {
            LogError("BaseComunicacaoServidor.ProcessRequestAsync", ex);
            throw;
        }
        catch (Exception ex)
        {
            LogError("BaseComunicacaoServidor.ProcessRequestAsync", ex);
            throw;
        }
        finally
        {
            this.LogTiming(()=> $"[BaseComunicacaoServidor.ProcessRequestAsync] Total ProcessRequestAsync - Tempo: {sw.ElapsedMilliseconds} ms", sw);
        }
    }
     
    protected void LogTiming(
        Func<string> value,
        Stopwatch stopwatch)
    {
        stopwatch.Stop();

        var info = this.Info;
        Func<string> messageFactory = () =>
            $"{value()} - Tempo: {stopwatch.ElapsedMilliseconds} ms\r\n" +
            $"Info: {info?.BuildInfo(newLineSeparator: false)}";

        
        TraceUtil.Timing(
            messageFactory,
            stopwatch);

        if(stopwatch.ElapsedMilliseconds > 1500)
        {
            LogUtil.DesempenhoAsync(
                messageFactory(),
                stopwatch,
                EnumTipoLogDesempenho.LentidaoServicoComunicacao,
                erroIsAttach: false);
        }
    }

    private void LogError(string identificador, Exception ex)
    {
        LogUtil.ErroAsync(new ErroRequisicao(ex, this.Info));
    }
}

