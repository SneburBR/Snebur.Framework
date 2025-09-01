using System.Runtime.CompilerServices;

namespace Snebur.Imagens;

[Serializable()]
public class ErroDimensaoImagem : Erro
{
    protected override bool IsNotificarErro { get; } = false;

    public ErroDimensaoImagem(string mensagem = "",
                          Exception? erroInterno = null,
                          [CallerMemberName] string nomeMetodo = "",
                          [CallerFilePath] string caminhoArquivo = "",
                          [CallerLineNumber] int linhaDoErro = 0) :
                          base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
    {

    }

    #region Serializacao 

    public ErroDimensaoImagem()
    {

    }

    #endregion
}
