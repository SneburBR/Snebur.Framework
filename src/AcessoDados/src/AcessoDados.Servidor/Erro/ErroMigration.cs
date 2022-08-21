﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Snebur.AcessoDados
{
    [Serializable]
    public class ErroMigracao : ErroAcessoDados
    {
        public ErroMigracao(string mensagem = "",
                             Exception erroInterno = null,
                             [CallerMemberName] string nomeMetodo = "",
                             [CallerFilePath] string caminhoArquivo = "",
                             [CallerLineNumber] int linhaDoErro = 0) : 
                              base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {
        }
        #region Serializacao 

        public ErroMigracao()
        {
        }

        protected ErroMigracao(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion
    }
}