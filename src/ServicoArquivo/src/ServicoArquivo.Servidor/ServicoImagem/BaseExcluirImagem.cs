#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif  

namespace Snebur.ServicoArquivo;


public abstract class BaseExcluirImagem : BaseExcluirArquivo<CabecalhoServicoImagem, IInformacaoRepositorioImagem>
{

    protected ComunicacaoServicoImagemCliente ServicoImagemCliente
    {
        get { return (ComunicacaoServicoImagemCliente)this.ServicoArquivoCliente; }
    }

    protected override ComunicacaoServicoArquivoCliente RetornarServicoArquivoCliente(CabecalhoServicoImagem cabecalho)
    {
        var urlServicoArquivo = this.RetornarUrlServicoArquivoCliente();
        return new ComunicacaoServicoImagemCliente(urlServicoArquivo,
                                                   cabecalho.CredencialRequisicao,
                                                   cabecalho.IdentificadorSessaoUsuario,
                                                   cabecalho.IdentificadorProprietario,
                                                   this.NormalizarOrigem);
    }

    protected override void ExcluirArquivo(HttpContext httpContext)
    {
        var cabecalho = this.RetornarCabecalhoServicoArquivo(httpContext) as CabecalhoServicoImagem;
        if (cabecalho == null)
        {
            throw new Exception(String.Format("O cabeçalho não foi definido."));
        }
        if (!(cabecalho.IdArquivo > 0))
        {
            throw new Exception(String.Format("O IdBaseStream deve ser maior 0. '{0}", cabecalho.IdArquivo));
        }
        var idArquivo = cabecalho.IdArquivo;
        var tamanhos = EnumUtil.RetornarValoresEnum<EnumTamanhoImagem>();

        foreach (EnumTamanhoImagem tamanhoImagem in tamanhos)
        {
            var caminhoImagem = this.RetornarCaminhoCompletoArquivo(cabecalho);
            if (File.Exists(caminhoImagem))
            {
                ArquivoUtil.DeletarArquivo(caminhoImagem, false, true);
            }
        }
        this.NotificarExcluirImagem(idArquivo);

#if NET6_0_OR_GREATER
        throw new NotImplementedException();
#else
        httpContext.Response.Write("true");
#endif

    }

    private void NotificarExcluirImagem(long IdBaseStream)
    {
        this.ServicoImagemCliente.NotificarArquivoDeletado(IdBaseStream);
    }

    #region  Métodos sobre-escritos

    protected override CabecalhoServicoImagem RetornarCabecalhoServicoArquivo(HttpContext httpContext)
    {
        return new CabecalhoServicoImagem(httpContext);
    }

    protected override sealed string RetornarDiretorioArquivo(IInformacaoRepositorioImagem informacaoRepositorio)
    {
        return this.RetornarDiretorioImagem(informacaoRepositorio);
    }

    protected override sealed string RetornarRepositoArquivo(IInformacaoRepositorioImagem informacaoRepositorio)
    {
        return this.RetornarRepositorioImagem(informacaoRepositorio);
    }

    public string RetornarDiretorioImagem(IInformacaoRepositorioImagem informacaoRepositorio)
    {
        return ServicoArquivoUtil.RetornarCaminhoDiretorioArquivo(this.RetornarRepositorioImagem(informacaoRepositorio), informacaoRepositorio.IdArquivo);
    }

    protected override string RetornarCaminhoCompletoArquivo(IInformacaoRepositorioImagem informacaoRepositorio)
    {
        if (informacaoRepositorio is not CabecalhoServicoImagem cabecalhoServicoImagem)
        {
            throw new Exception($"O {nameof(informacaoRepositorio)} deve ser do tipo {nameof(CabecalhoServicoImagem)}.");
        }
        var tamanhoImagem = cabecalhoServicoImagem.TamanhoImagem;
        var caminhoDiretorioImagem = this.RetornarDiretorioImagem(informacaoRepositorio);
        var nomeArquivo = ServicoArquivoUtil.RetornarNomeArquivo(informacaoRepositorio.IdArquivo, ServicoImagemUtil.RetornarExtensaoImagem(tamanhoImagem));
        return Path.Combine(caminhoDiretorioImagem, nomeArquivo);
    }
    #endregion

    protected abstract string RetornarRepositorioImagem(IInformacaoRepositorioImagem informacaoRepositorio);
}