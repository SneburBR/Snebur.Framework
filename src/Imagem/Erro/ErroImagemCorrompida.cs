using System.Runtime.CompilerServices;

namespace Snebur.Imagens;

[Serializable()]
public class ErroImagemCorrompida : Erro
{
    protected override bool IsNotificarErro { get; } = false;

    public ErroImagemCorrompida(string mensagem = "",
                               Exception? erroInterno = null,
                               [CallerMemberName] string nomeMetodo = "",
                               [CallerFilePath] string caminhoArquivo = "",
                               [CallerLineNumber] int linhaDoErro = 0) :
                               base(mensagem,
                                   erroInterno,
                                   nomeMetodo,
                                   caminhoArquivo,
                                   linhaDoErro)
    {

    }

    #region Serializacao 

    public ErroImagemCorrompida()
    {
        this.IsNotificarErro = false;
    }

    #endregion
}
