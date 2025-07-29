using Snebur;
using System.Runtime.CompilerServices;

namespace Snebur;

[Serializable]
public class ErroGlobal : Erro
{

    public ErroGlobal(string mensagem = "",
                     Exception? erroInterno = null,
                     [CallerMemberName] string nomeMetodo = "",
                     [CallerFilePath] string caminhoArquivo = "",
                     [CallerLineNumber] int linhaDoErro = 0) :
                     base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
    {
    }
   
#if !EXTENSAO_VISUALSTUDIO

    protected override EnumNivelErro RetornarNivelErro()
    {
        return EnumNivelErro.Critico;
    }
#endif
}