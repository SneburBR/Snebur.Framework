using System.IO;
using System.Text;
using Snebur.Net;
using Snebur.Utilidade;

namespace Snebur.ServicoArquivo
{
    public abstract class BaseBaixarArquivo<TCabecalhoServicoArquivo, TInformacaoRepositorioArquivo> : BaseServicoArquivo<TCabecalhoServicoArquivo, TInformacaoRepositorioArquivo> where TCabecalhoServicoArquivo : CabecalhoServicoArquivo, TInformacaoRepositorioArquivo
                                                                                                                                                                                   where TInformacaoRepositorioArquivo : IInformacaoRepositorioArquivo
    {
        public override bool IsReponserJson => false;

        protected override void Iniciar(ZyonHttpContext context, TCabecalhoServicoArquivo cabecalho)
        {
            this.BaixarArquivo(context, cabecalho);
        }

        protected virtual void BaixarArquivo(ZyonHttpContext context, TCabecalhoServicoArquivo cabecalho)
        {
            if (cabecalho.IsCabecalhoValido())
            {
                var caminhoCompletoArquivo = this.RetornarCaminhoCompletoArquivo(cabecalho);
                if (File.Exists(caminhoCompletoArquivo))
                {
                    context.Response.ContentType = "application/octet-stream";
                    context.Response.ContentEncoding = Encoding.UTF8;
                    context.Response.WriteFile(caminhoCompletoArquivo);
                    return;
                }
                else
                {
                    LogUtil.ErroAsync(new ErroArquivoNaoEncontrado(caminhoCompletoArquivo));
                }
            }

            context.Response.SubStatusCode = 5;
            context.Response.StatusCode = 405;
         
        }
    }

    public abstract class BaseBaixarArquivo : BaseBaixarArquivo<CabecalhoServicoArquivo, IInformacaoRepositorioArquivo>
    {

    }
}