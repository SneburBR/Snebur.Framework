using System.Runtime.CompilerServices;

namespace Snebur.AcessoDados;

[Serializable]
public class ErroSessaoUsuarioExpirada : Erro
{
    internal protected override bool IsParaDepuracaoAtachada => false;

    public EnumStatusSessaoUsuario StatusSessao { get; }

    public ErroSessaoUsuarioExpirada(
        EnumStatusSessaoUsuario status,
        Guid identificadorSessaoUsuario,
        string mensagem = "",
        Exception? erroInterno = null,
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
     
    protected override bool IsNotificar()
    {
        return false;
    }
    #endregion
}

public class ErroSessaoUsuarioInvalida : Erro
{
    public ErroSessaoUsuarioInvalida(string mensagem = "",
                                    Exception? erroInterno = null,
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
     
    protected override bool IsNotificar()
    {
        return false;
    }
    #endregion
}