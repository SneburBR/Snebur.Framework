﻿using System.Runtime.CompilerServices;

namespace Snebur;

[Serializable]
public class ErroExcluirArquivo : Erro
{

    public ErroExcluirArquivo(string mensagem = "",
                              Exception? erroInterno = null,
                              [CallerMemberName] string nomeMetodo = "",
                              [CallerFilePath] string caminhoArquivo = "",
                              [CallerLineNumber] int linhaDoErro = 0) :
                              base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
    {
    }

    #region Serializacao 

    public ErroExcluirArquivo()
    {
    }
     
    #endregion
}