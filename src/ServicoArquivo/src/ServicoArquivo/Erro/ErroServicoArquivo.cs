using System.Runtime.CompilerServices;

namespace Snebur.ServicoArquivo;

[Serializable]
public abstract class ErroServicoArquivo : Snebur.Erro
{
    public ErroServicoArquivo(string mensagem = "",
                                Exception? erroInterno = null,
                                [CallerMemberName] string nomeMetodo = "",
                                [CallerFilePath] string caminhoArquivo = "",
                                [CallerLineNumber] int linhaDoErro = 0) :
                                 base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
    {
    }
    #region Serializacao 

    public ErroServicoArquivo()
    {
    }

    #endregion
}