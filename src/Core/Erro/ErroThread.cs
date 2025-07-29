using Snebur;
using System.Runtime.CompilerServices;

namespace Snebur;

[Serializable]
public class ErroThread : Erro
{

    public ErroThread(string mensagem = "",
                     Exception? erroInterno = null,
                     [CallerMemberName] string nomeMetodo = "",
                     [CallerFilePath] string caminhoArquivo = "",
                     [CallerLineNumber] int linhaDoErro = 0) :
                     base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
    {
    }
    #region Serializacao 
     
    protected override EnumNivelErro RetornarNivelErro()
    {
        return EnumNivelErro.Baixo;
    }
    #endregion
}