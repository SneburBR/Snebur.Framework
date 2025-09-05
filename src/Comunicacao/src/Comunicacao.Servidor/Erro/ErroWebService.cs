using System.Runtime.CompilerServices;

namespace Snebur.Comunicacao;

public class ErroWebService : ErroComunicacao
{
    public ErroWebService(Exception erroInterno,
                          string nomeManipulador,
                          [CallerMemberName] string nomeMetodo = "",
                          [CallerFilePath] string caminhoArquivo = "",
                          [CallerLineNumber] int linhaDoErro = 0) :
                          base(erroInterno.Message,
                                  erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
    {

    }
}
