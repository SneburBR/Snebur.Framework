using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System
{
    [Serializable]
    public abstract class ErroServicoArquivo : Erro
    {
        public ErroServicoArquivo(string mensagem = "",
                                    Exception erroInterno = null,
                                    [CallerMemberName] string nomeMetodo = "",
                                    [CallerFilePath] string caminhoArquivo = "",
                                    [CallerLineNumber] int linhaDoErro = 0) :
                                     base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {
        }
        #region Serializacao 

        public ErroServicoArquivo()
        {
        }

        protected ErroServicoArquivo(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion
    }
}