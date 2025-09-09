using System.Runtime.CompilerServices;

namespace Snebur.AcessoDados;

[Serializable]
public class ErroValidacao : ErroAcessoDados
{

    public ErroValidacao(List<Snebur.Dominio.ErroValidacao> erros,
                         Exception? erroInterno = null,
                         [CallerMemberName] string nomeMetodo = "",
                         [CallerFilePath] string caminhoArquivo = "",
                         [CallerLineNumber] int linhaDoErro = 0) :
                         base(RetornarMensagemErro(erros),
                             erroInterno,
                             nomeMetodo,
                             caminhoArquivo,
                             linhaDoErro)
    {
    }

    public static string RetornarMensagemErro(List<Snebur.Dominio.ErroValidacao> erros)
    {
        return String.Join("\n", erros.Select(x => x.Mensagem));
    }
    #region Serializacao 

    public ErroValidacao()
    {
    }

    #endregion
}