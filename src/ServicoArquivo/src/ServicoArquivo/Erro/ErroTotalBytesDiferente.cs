using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System
{
    [Serializable]
    public class ErroTotalBytesDiferente : ErroServicoArquivo
    {

        public ErroTotalBytesDiferente(string mensagem = "",
                                    Exception? erroInterno = null,
                                    [CallerMemberName] string nomeMetodo = "",
                                    [CallerFilePath] string caminhoArquivo = "",
                                    [CallerLineNumber] int linhaDoErro = 0) :
                                     base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {
        }
        #region Serializacao 

        public ErroTotalBytesDiferente()
        {
        }
         
        #endregion
    }
}