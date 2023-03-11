#if NET7_0
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif  


namespace Snebur.ServicoArquivo
{

    public abstract class BaseExcluirArquivo<TCabecalhoServicoArquivo, TInformacaoRepositorio> : BaseServicoArquivo<TCabecalhoServicoArquivo, TInformacaoRepositorio>
                                                                                                 where TCabecalhoServicoArquivo : CabecalhoServicoArquivo, TInformacaoRepositorio
                                                                                                 where TInformacaoRepositorio : IInformacaoRepositorioArquivo
    {
        protected abstract void ExcluirArquivo(HttpContext context);
    }
}