using System.Runtime.CompilerServices;

namespace Snebur.Tarefa;

[Serializable]
public class ErroTarefa : Erro
{
    public ErroTarefa(string mensagem = "",
                      Exception? erroInterno = null,
                      [CallerMemberName] string nomeMetodo = "",
                      [CallerFilePath] string caminhoArquivo = "",
                      [CallerLineNumber] int linhaDoErro = 0) :
                      base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
    {
    }

    #region Serializacao 

    public ErroTarefa()
    {
    }
     
    #endregion
}