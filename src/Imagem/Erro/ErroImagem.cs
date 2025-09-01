using System.Runtime.CompilerServices;

namespace Snebur.Imagens;

[Serializable()]
public class ErroImagem : Erro
{

    public ErroImagem(string mensagem = "",
                          Exception? erroInterno = null,
                          [CallerMemberName] string nomeMetodo = "",
                          [CallerFilePath] string caminhoArquivo = "",
                          [CallerLineNumber] int linhaDoErro = 0) :
                          base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
    {

    }

    #region Serializacao 

    public ErroImagem()
    {

    }

    #endregion
}
