using System.Runtime.CompilerServices;

namespace Snebur.AcessoDados;

[Serializable]
public class ErroFalhaManutencao : ErroAcessoDados
{
    public ErroFalhaManutencao(string mensagem = "",
        Exception? erroInterno = null,
        [CallerMemberName] string nomeMetodo = "",
        [CallerFilePath] string caminhoArquivo = "",
        [CallerLineNumber] int linhaDoErro = 0) :
        base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
    {
    }
    #region Serializacao 

    public ErroFalhaManutencao()
    {
    }

    #endregion
}