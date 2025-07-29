using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System
{
    [Serializable]
    public class ErroArquivoEmUso : ErroServicoArquivo
    {
        public ErroArquivoEmUso(string mensagem = "",
                                     Exception? erroInterno = null,
                                     [CallerMemberName] string nomeMetodo = "",
                                     [CallerFilePath] string caminhoArquivo = "",
                                     [CallerLineNumber] int linhaDoErro = 0) :
                                     base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {
        }
        #region Serializacao 

        public ErroArquivoEmUso()
        {
        }
         
        #endregion
    }
}