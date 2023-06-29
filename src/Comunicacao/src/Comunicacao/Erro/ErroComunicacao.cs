using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Snebur.Comunicacao
{
    [Serializable]
    public class ErroComunicacao : Erro
    {
        public ErroComunicacao(string mensagem = "",
                                  Exception erroInterno = null,
                                  [CallerMemberName] string nomeMetodo = "",
                                  [CallerFilePath] string caminhoArquivo = "",
                                  [CallerLineNumber] int linhaDoErro = 0) :
                                        base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {
        }

        #region Serializacao 

        public ErroComunicacao()
        {
        }

        protected ErroComunicacao(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion
    }
}