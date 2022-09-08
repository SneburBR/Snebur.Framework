using System;
using System.Runtime.CompilerServices;

namespace Snebur.Comunicacao
{
    public class ErroReceberStreamCliente : ErroComunicacao
    {
        public ErroReceberStreamCliente(string mensagem = "",
                                       Exception erroInterno = null,
                                       [CallerMemberName] string nomeMetodo = "",
                                       [CallerFilePath] string caminhoArquivo = "",
                                       [CallerLineNumber] int linhaDoErro = 0) :
                                       base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {
        }
    }
}
