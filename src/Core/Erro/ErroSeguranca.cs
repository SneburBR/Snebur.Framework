using Snebur.Servicos;
using System.Runtime.CompilerServices;

namespace Snebur;

public class ErroSeguranca : Erro
{
    public EnumTipoLogSeguranca TipoLogSeguranca { get; }
    public ErroSeguranca(string mensagem,
                         EnumTipoLogSeguranca tipo,
                         Exception? erroInterno = null,
                         [CallerMemberName] string nomeMetodo = "",
                         [CallerFilePath] string caminhoArquivo = "",
                         [CallerLineNumber] int linhaDoErro = 0) :
                         base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
    {
        this.TipoLogSeguranca = tipo;
#if !EXTENSAO_VISUALSTUDIO

        LogUtil.SegurancaAsync(mensagem, tipo, false);

#endif
    }
    #region Serializacao 

    public ErroSeguranca()
    {
    }

    #endregion
}
