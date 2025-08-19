using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Snebur.AcessoDados
{
    [Serializable]
    public class ErroSenhaIncorreta : ErroAcessoDados
    {
        public ErroSenhaIncorreta(string mensagem = "",
                                       Exception erroInterno = null,
                                       [CallerMemberName] string nomeMetodo = "",
                                       [CallerFilePath] string caminhoArquivo = "",
                                       [CallerLineNumber] int linhaDoErro = 0) :
                                        base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {
        }
        #region Serializacao 

        public ErroSenhaIncorreta()
        {
        }
         
        #endregion
    }
}