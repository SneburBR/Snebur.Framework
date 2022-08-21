using System;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;

namespace Snebur.ServicoArquivo
{
    [Serializable]
    public class ErroIdArquivoNaoExiste : ErroServicoArquivo
    {

        public ErroIdArquivoNaoExiste(string mensagem = "",
                                        Exception erroInterno = null,
                                        [CallerMemberName] string nomeMetodo = "",
                                        [CallerFilePath] string caminhoArquivo = "",
                                        [CallerLineNumber] int linhaDoErro = 0) :
                                        base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {
        }
        #region Serializacao 

        public ErroIdArquivoNaoExiste()
        {
        }

        protected ErroIdArquivoNaoExiste(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion
    }

    
}