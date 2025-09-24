using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Snebur.AcessoDados;

[Serializable]
public abstract class ErroAcessoDados : Erro
{
    protected override bool IsParaDepuracaoAtachada => false;
    public ErroAcessoDados(string mensagem = "",
                           Exception? erroInterno = null,
                           [CallerMemberName] string nomeMetodo = "",
                           [CallerFilePath] string caminhoArquivo = "",
                           [CallerLineNumber] int linhaDoErro = 0) : base(mensagem, erroInterno)
    {
        Debugger.Break();
    }

    #region Serializacao 

    public ErroAcessoDados()
    {
    }

    public static ErroAcessoDados Create(
        Exception erro,
        [CallerMemberName] string nomeMetodo = "",
        [CallerFilePath] string caminhoArquivo = "",
        [CallerLineNumber] int linhaDoErro = 0)
    {
        if (erro is ErroAcessoDados erroAcessoDados)
        {
            return erroAcessoDados;
        }
        return new ErroAcessoDadosGenerico(erro.Message, erro, nomeMetodo, caminhoArquivo, linhaDoErro);
    }

    #endregion
}

public class ErroAcessoDadosGenerico : ErroAcessoDados
{
    public ErroAcessoDadosGenerico(string mensagem = "",
                           Exception? erroInterno = null,
                           [CallerMemberName] string nomeMetodo = "",
                           [CallerFilePath] string caminhoArquivo = "",
                           [CallerLineNumber] int linhaDoErro = 0) : base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
    {
    }
    #region Serializacao 
    public ErroAcessoDadosGenerico()
    {
    }
    #endregion
}