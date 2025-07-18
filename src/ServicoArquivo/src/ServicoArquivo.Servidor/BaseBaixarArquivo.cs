using Snebur.Utilidade;
using System.IO;
using System.Text;
using System;
using System.Threading.Tasks;

#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif  

namespace Snebur.ServicoArquivo
{
    public abstract class BaseBaixarArquivo<TCabecalhoServicoArquivo, TInformacaoRepositorioArquivo> : BaseServicoArquivo<TCabecalhoServicoArquivo, TInformacaoRepositorioArquivo> where TCabecalhoServicoArquivo : CabecalhoServicoArquivo, TInformacaoRepositorioArquivo
                                                                                                                                                                                   where TInformacaoRepositorioArquivo : IInformacaoRepositorioArquivo
    {
        public override bool IsReponserJson => false;

#if NET6_0_OR_GREATER

        protected override async Task IniciarAsync(HttpContext context, TCabecalhoServicoArquivo cabecalho, MemoryStream inputStream)
        {
            await this.BaixarArquivoAsync(context, cabecalho);
        }

          protected virtual async Task BaixarArquivoAsync(HttpContext context,
                                                          TCabecalhoServicoArquivo cabecalho)
        {
            var caminhoCompletoArquivo = this.RetornarCaminhoCompletoArquivo(cabecalho);
            if (!cabecalho.IsCabecalhoValido())
            {
                context.Response.StatusCode = 405;
                return;
            }

            if (File.Exists(caminhoCompletoArquivo))
            {
                LogUtil.ErroAsync(new ErroArquivoNaoEncontrado(caminhoCompletoArquivo));
                context.Response.StatusCode = 500;
                return;
            }

            var nomeArquivo = cabecalho.IdArquivo + ".bin";
            var response = context.Response;
            response.ContentType = "application/octet-stream";
            response.Headers.Append("Content-Disposition", $"attachment; filename=\"{nomeArquivo}\"");

            using (var fs = StreamUtil.OpenRead(caminhoCompletoArquivo))
            {
                await StreamUtil.SalvarStreamBufferizadaAsync(fs, response.Body);
            }
        }

#else
        protected override void Iniciar(HttpContext context, TCabecalhoServicoArquivo cabecalho, MemoryStream inputStream)
        {
            this.BaixarArquivo(context, cabecalho);
        }

        protected virtual void BaixarArquivo(HttpContext context,
                                          TCabecalhoServicoArquivo cabecalho)
        {
            var caminhoCompletoArquivo = this.RetornarCaminhoCompletoArquivo(cabecalho);
            if (!cabecalho.IsCabecalhoValido())
            {
                context.Response.StatusCode = 405;
                return;
            }

            if (File.Exists(caminhoCompletoArquivo))
            {
                LogUtil.ErroAsync(new ErroArquivoNaoEncontrado(caminhoCompletoArquivo));
                context.Response.StatusCode = 500;
                return;
            }

            context.Response.ContentType = "application/octet-stream";
            context.Response.WriteFile(caminhoCompletoArquivo);
        }

#endif

    }

    public abstract class BaseBaixarArquivo : BaseBaixarArquivo<CabecalhoServicoArquivo, IInformacaoRepositorioArquivo>
    {

    }
}