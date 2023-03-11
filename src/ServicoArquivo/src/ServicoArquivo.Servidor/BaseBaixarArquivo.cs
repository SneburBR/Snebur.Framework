using Snebur.Utilidade;
using System.IO;
using System.Text;
using System;

#if NET7_0
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

        protected override void Iniciar(HttpContext context, TCabecalhoServicoArquivo cabecalho, MemoryStream inputStream)
        {
            this.BaixarArquivo(context, cabecalho);
        }

        protected virtual void BaixarArquivo(HttpContext context, TCabecalhoServicoArquivo cabecalho)
        {
            if (cabecalho.IsCabecalhoValido())
            {
                var caminhoCompletoArquivo = this.RetornarCaminhoCompletoArquivo(cabecalho);
                if (File.Exists(caminhoCompletoArquivo))
                {
                    context.Response.ContentType = "application/octet-stream";
#if NET7_0
                    throw new NotImplementedException();
                    //using (var fs = StreamUtil.OpenRead(caminhoCompletoArquivo))
                    //{
                    //    await StreamUtil.SalvarStreamBufferizadaAsync(fs, context.Response.Body);
                    //}
#else
                    context.Response.WriteFile(caminhoCompletoArquivo);
#endif

                    return;
                }
                else
                {
                    LogUtil.ErroAsync(new ErroArquivoNaoEncontrado(caminhoCompletoArquivo));
                }
            }

            context.Response.StatusCode = 405;

        }
    }

    public abstract class BaseBaixarArquivo : BaseBaixarArquivo<CabecalhoServicoArquivo, IInformacaoRepositorioArquivo>
    {

    }
}