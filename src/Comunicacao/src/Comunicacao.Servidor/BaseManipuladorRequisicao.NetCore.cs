
#if NET6_0_OR_GREATER

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Snebur.Servicos;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Snebur.Comunicacao;

public abstract partial class BaseManipuladorRequisicao : IHttpModule, IDisposable
{
    public bool IsWebSocket { get; protected set; }

    public IConfigurationRoot Configuration => throw new NotImplementedException();

    public void ConfigureServices(IServiceCollection services)
    {
        Startup.ConfigureServicesInterno(services);
    }

    public async Task ProcessarRequisicaoAsync(HttpContext context)
    {
        if (CrossDomainUtil.VerificarContratoCrossDomain(context))
        {
            await context.CompleteRequestAsync();
            return;
        }
        this.AntesProcessarRequisicao(context);

        var request = context.Request;
        var response = context.Response;

        if (await this.IsExecutarServicoAsync(context))
        {
            response.StatusCode = 0;
            try
            {
                await this.ExecutarServicoAsync(context);
                //Chamadas do cross domain do ajax serão implementas aqui
            }
            catch (Exception ex)
            {

                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                if (DebugUtil.IsAttached)
                {
                    throw;
                }
                LogUtil.ErroAsync(ex);
            }
            finally
            {
                if (response.StatusCode == 0)
                {
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                }
                await context.CompleteRequestAsync();
            }
        }
    }

    private async Task ExecutarServicoAsync(HttpContext httpContext)
    {
        var request = httpContext.Request;

        if (this.IsRequicaoValida(request, true))
        {
            var identificadorProprietario = this.RetornarIdentificadorProprietario(request);
            if (!request.Headers.TryGetValue(ParametrosComunicacao.MANIPULADOR, out var manipuladorValues) ||
                manipuladorValues.Count == 0 || String.IsNullOrWhiteSpace(manipuladorValues))
            {
                this.NotificarLogSeguranca(request, EnumTipoLogSeguranca.ManipuladorNaoDefinido);
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            string nomeManipulador = manipuladorValues.ToString();
            var tipoManipulador = this.RetornarTipoServico(nomeManipulador);
            if (tipoManipulador is null)
            {
                this.NotificarServicoNaoEncontado(httpContext, nomeManipulador);
                return;
            }

            var comicacaoServidor = Activator.CreateInstance(tipoManipulador) as BaseComunicacaoServidor
                ?? throw new ErroComunicacao($"O manipulador {nomeManipulador} não é um {nameof(BaseComunicacaoServidor)} válido.");

            using (var servico = comicacaoServidor)
            {
                try
                {
                    servico.IdentificadorProprietario = identificadorProprietario;
                    await servico.ProcessRequestAsync(httpContext);
                }
                catch (ErroRequisicao)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ErroWebService(ex, nomeManipulador ?? "manipulador não definido");
                }
            }
        }
    }

    private async Task<bool> IsExecutarServicoAsync(HttpContext httpContext)
    {
        var request = httpContext.Request;
        var response = httpContext.Response;

        var caminho = Path.GetFileName(request.RetornarUrlRequisicao().LocalPath).ToLower();

        if (CrossDomainUtil.VerificarContratoCrossDomain(httpContext))
        {
            await httpContext.CompleteRequestAsync();
            return false;
        }

        if (caminho == "/" || caminho == "")
        {
            await this.ResponderAgoraAsync(response);
            await httpContext.CompleteRequestAsync();
            return false;
        }

        if (caminho.Equals("agora", StringComparison.InvariantCultureIgnoreCase))
        {
            await this.ResponderAgoraAsync(response);
            await httpContext.CompleteRequestAsync();
            return false;
        }

        if (caminho.Equals("ping", StringComparison.InvariantCultureIgnoreCase))
        {
            await response.WriteAsync("True", Encoding.UTF8);
            await httpContext.CompleteRequestAsync();
            return false;
        }

        var caminhoManipulador = Path.GetFileNameWithoutExtension(caminho);
        if (this.ManipuladoresGenericos.ContainsKey(caminhoManipulador))
        {
            var isValidarToken = this.ManipuladoresGenericos[caminhoManipulador].isValidarToken;
            if (isValidarToken && !this.IsRequicaoValida(request, false))
            {
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await httpContext.CompleteRequestAsync();
                return false;
            }

            await this.ExecutarManipuladorGenericoAsync(httpContext, caminhoManipulador);
            await httpContext.CompleteRequestAsync();
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
                    await httpContext.CompleteRequestAsync();
                    return false;
                }
            }
            return false;
        }

        if ((request.Headers.ContainsKey(ParametrosComunicacao.TOKEN) &&
            !request.Headers.ContainsKey("TOKEN")))
        {
            throw new Exception("Invalida case insensitive");
            //await httpContext.CompleteRequestAsync();
            //return false;
        }

        if (!request.Headers.ContainsKey(ParametrosComunicacao.TOKEN) ||
            !request.Headers.ContainsKey(ParametrosComunicacao.MANIPULADOR))
        {

            LogUtil.SegurancaAsync($"A URL '{request.RetornarUrlRequisicao().AbsoluteUri}' foi chamada incorretamente.", Servicos.EnumTipoLogSeguranca.CabecalhoInvalido);
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            await httpContext.CompleteRequestAsync();
            return false;
        }
        return true;
    }

    private async Task ResponderAgoraAsync(HttpResponse response)
    {
        var agora = DateTime.UtcNow.AddSeconds(-10);
        await this.ResponderAsync(response, agora.Ticks.ToString());
    }

    private async Task ResponderAsync(HttpResponse response, string conteuto)
    {
        response.ContentType = "text/text; charset=UTF-8";
        if (!response.HasStarted)
        {
            response.StatusCode = 200;
        }
        await response.WriteAsync(conteuto, Encoding.UTF8);
    }
    private async Task ExecutarManipuladorGenericoAsync(HttpContext httpContext, string caminho)
    {
        var tipo = this.ManipuladoresGenericos[caminho].tipo;
        var manipualador = Activator.CreateInstance(tipo) as IHttpHandler;
        if (manipualador != null)
        {
            await manipualador.ProcessRequestAsync(httpContext);
        }
    }

    private static async Task Echo(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];

        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!receiveResult.CloseStatus.HasValue)
        {
            await webSocket.SendAsync(
                new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                receiveResult.MessageType,
                receiveResult.EndOfMessage,
                CancellationToken.None);

            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }
}

public class AshxHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public AshxHandlerMiddleware(RequestDelegate next)
    {
        this._next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.Value?.EndsWith(".ashx") == true)
        {
            await HandleAshxRequest(context);
        }
        else
        {
            await this._next(context);
        }
    }

    private Task HandleAshxRequest(HttpContext context)
    {
        // Lógica do seu handler .ashx aqui
        context.Response.ContentType = "text/plain";
        return context.Response.WriteAsync("Hello from .ashx handler!");
    }
}

public static class AshxHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseAshxHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AshxHandlerMiddleware>();
    }
}

#endif