using Snebur.Dominio;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Snebur.AcessoDados
{
    [Serializable]
    public class ErroSessaoUsuarioExpirada : Erro
    {
        internal protected override bool IsParaDepuracaoAtachada => false;

        public EnumEstadoSessaoUsuario EstadoSessao { get; }

        public ErroSessaoUsuarioExpirada(EnumEstadoSessaoUsuario estado,
                                        Guid identificadorSessaoUsuario,
                                        string mensagem = "",
                                        Exception erroInterno = null,
                                        [CallerMemberName] string nomeMetodo = "",
                                        [CallerFilePath] string caminhoArquivo = "",
                                        [CallerLineNumber] int linhaDoErro = 0) :
                                        base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {
        }
        #region Serializacao 

        public ErroSessaoUsuarioExpirada()
        {
        }

        protected ErroSessaoUsuarioExpirada(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        protected override bool IsNotificar()
        {
            return false;
        }
        #endregion
    }

    public class ErroSessaoUsuarioInvalida : Erro
    {
        public ErroSessaoUsuarioInvalida(string mensagem = "",
                                        Exception erroInterno = null,
                                        [CallerMemberName] string nomeMetodo = "",
                                        [CallerFilePath] string caminhoArquivo = "",
                                        [CallerLineNumber] int linhaDoErro = 0) :
                                        base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {
        }
        #region Serialização 

        public ErroSessaoUsuarioInvalida()
        {
        }

        protected ErroSessaoUsuarioInvalida(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        protected override bool IsNotificar()
        {
            return false;
        }
        #endregion
    }
}