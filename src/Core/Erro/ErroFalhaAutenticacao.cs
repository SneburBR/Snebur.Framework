﻿using System.Runtime.CompilerServices;

namespace Snebur.AcessoDados;

[Serializable]
public class ErroFalhaAutenticacao : Erro
{
    public ErroFalhaAutenticacao(string mensagem = "",
                                 Exception? erroInterno = null,
                                 [CallerMemberName] string nomeMetodo = "",
                                 [CallerFilePath] string caminhoArquivo = "",
                                 [CallerLineNumber] int linhaDoErro = 0) :
                                 base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
    {
    }
    #region Serializacao 

    public ErroFalhaAutenticacao()
    {
    }
     
    #endregion
}