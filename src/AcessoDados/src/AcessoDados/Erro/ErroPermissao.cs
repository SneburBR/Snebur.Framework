using Snebur.AcessoDados.Seguranca;
using System.Runtime.CompilerServices;

namespace Snebur.AcessoDados;

[Serializable]
public class ErroPermissao : ErroAcessoDados
{
    public ErroPermissao(EnumPermissao permissao, string mensagem,
        Exception? erroInterno = null,
        [CallerMemberName] string nomeMetodo = "",
        [CallerFilePath] string caminhoArquivo = "",
        [CallerLineNumber] int linhaDoErro = 0) :
         base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
    {
    }
    #region Serializacao 

    public ErroPermissao()
    {
    }

    #endregion
}