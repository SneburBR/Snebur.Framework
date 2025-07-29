using System.Runtime.CompilerServices;

namespace Snebur;

public class ErroCritico : Erro
{
    //public EnumTipoErroCritico TipoErroCritico { get; }
    public ErroCritico(string mensagem,
                       Exception? erroInterno = null,
                       [CallerMemberName] string nomeMetodo = "",
                       [CallerFilePath] string caminhoArquivo = "",
                       [CallerLineNumber] int linhaDoErro = 0) :
                       base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
    {
        //this.TipoErroCritico = tipo;
    }
    #region Serializacao 

    public ErroCritico()
    {
    }
     
    #endregion
}
