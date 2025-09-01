using System.Runtime.CompilerServices;

namespace Snebur.Imagens;

[Serializable()]
public class ErroMemoriaInsuficiente : Erro
{
    protected override bool IsNotificarErro { get; } = false;

    public ErroMemoriaInsuficiente(string mensagem = "",
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

    public ErroMemoriaInsuficiente()
    {
        this.IsNotificarErro = true;
    }

    #endregion
}
