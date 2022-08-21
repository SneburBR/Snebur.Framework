using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Snebur.AcessoDados.Seguranca;

namespace Snebur.AcessoDados
{
    [Serializable]
    public class ErroPermissao : ErroAcessoDados
    {
        public ErroPermissao(EnumPermissao permissao, string mensagem,
                             Exception erroInterno = null,
                             [CallerMemberName] string nomeMetodo = "",
                             [CallerFilePath] string caminhoArquivo = "",
                             [CallerLineNumber] int linhaDoErro = 0) :
                              base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {
        }
        #region Serializacao 

        public ErroPermissao()
        {
        }

        protected ErroPermissao(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion
    }
}