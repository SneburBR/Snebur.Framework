using Snebur.Net;

namespace Snebur.ServicoArquivo
{

    public abstract class BaseExcluirArquivo<TCabecalhoServicoArquivo, TInformacaoRepositorio> : BaseServicoArquivo<TCabecalhoServicoArquivo, TInformacaoRepositorio> 
                                                                                                 where TCabecalhoServicoArquivo : CabecalhoServicoArquivo, TInformacaoRepositorio
                                                                                                 where TInformacaoRepositorio: IInformacaoRepositorioArquivo
    {
    	protected abstract void ExcluirArquivo(SnHttpContext context);
    }
}   