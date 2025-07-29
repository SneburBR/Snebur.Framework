#if NET40
extern alias MicrosoftBcl;
#endif
using System.Runtime.CompilerServices;

namespace System;

[Serializable]
public class ErroArquivoNaoEncontrado : Erro
{

    public ErroArquivoNaoEncontrado(
        string mensagem = "",
        Exception? erroInterno = null,
        [CallerMemberName] string nomeMetodo = "",
        [CallerFilePath] string caminhoArquivo = "",
        [CallerLineNumber] int linhaDoErro = 0) :
        base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
    {
    }
    #region Serializacao 

    public ErroArquivoNaoEncontrado()
    {
    }
     
    #endregion
}