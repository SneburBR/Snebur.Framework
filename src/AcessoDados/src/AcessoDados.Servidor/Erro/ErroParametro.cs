using System.Runtime.CompilerServices;

namespace Snebur.AcessoDados;

public class ErroParametro : ErroAcessoDados
{
    public ErroParametro(string mensagem = "",
                          Exception? erroInterno = null,
                          [CallerMemberName] string nomeMetodo = "",
                          [CallerFilePath] string caminhoArquivo = "",
                          [CallerLineNumber] int linhaDoErro = 0) :
                          base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
    {
    }
    #region Serialização 

    public ErroParametro()
    {
    }

    #endregion
}