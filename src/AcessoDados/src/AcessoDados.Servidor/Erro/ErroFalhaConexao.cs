using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Snebur.AcessoDados
{
    [Serializable]
    public class ErroFalhaConexao : ErroAcessoDados
    {

        public ErroFalhaConexao(string mensagem = "",
                              Exception erroInterno = null,
                              [CallerMemberName] string nomeMetodo = "",
                              [CallerFilePath] string caminhoArquivo = "",
                              [CallerLineNumber] int linhaDoErro = 0) :
                              base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {
        }
        #region Serializacao 

        public ErroFalhaConexao()
        {
        }

        protected ErroFalhaConexao(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        protected override EnumNivelErro RetornarNivelErro()
        {
            return EnumNivelErro.Critico;
        }
        #endregion
    }
}