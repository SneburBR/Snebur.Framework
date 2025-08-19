using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Snebur.AcessoDados
{
    [Serializable]
    public abstract class ErroAcessoDados : Erro
    {
        protected override bool IsParaDepuracaoAtachada => false;
        public ErroAcessoDados(string mensagem = "",
                               Exception erroInterno = null,
                               [CallerMemberName] string nomeMetodo = "",
                               [CallerFilePath] string caminhoArquivo = "",
                               [CallerLineNumber] int linhaDoErro = 0) : base(mensagem, erroInterno)
        {
        }

        #region Serializacao 

        public ErroAcessoDados()
        {
        }
         
        #endregion
    }

}