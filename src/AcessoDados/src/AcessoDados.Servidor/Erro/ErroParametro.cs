using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Snebur.AcessoDados
{
    public class ErroParametro : ErroAcessoDados
    {
        public ErroParametro(string mensagem = "",
                              Exception erroInterno = null,
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

        public ErroParametro(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion
    }

}