using System.Runtime.CompilerServices;

namespace Snebur.Comunicacao;

public class ErroManipualdorNaoEncontrado : ErroComunicacao
{
    public ErroManipualdorNaoEncontrado(string mensagem,
                             [CallerMemberName] string nomeMetodo = "",
                             [CallerFilePath] string caminhoArquivo = "",
                             [CallerLineNumber] int linhaDoErro = 0) :
                               base(mensagem,
                                   null, nomeMetodo, caminhoArquivo, linhaDoErro)
    {

    }
}
