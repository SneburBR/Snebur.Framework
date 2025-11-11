using System.Runtime.CompilerServices;

namespace Snebur.Comunicacao;

public class ErroRequisicao : Erro
{
    public ComunicaoRequisicaoInfo? Info { get; }

    public ErroRequisicao(Exception erroInterno,
                         ComunicaoRequisicaoInfo? info,
                         [CallerMemberName] string nomeMetodo = "",
                         [CallerFilePath] string caminhoArquivo = "",
                         [CallerLineNumber] int linhaDoErro = 0) :
                         base(RetornarMensagemErro(erroInterno, info),
                              erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
    {
        this.Info = info;
    }

    private static string RetornarMensagemErro(Exception erro,  ComunicaoRequisicaoInfo? info)
    {
        return $"Info: {info?.BuildInfo()}";
    }
}
