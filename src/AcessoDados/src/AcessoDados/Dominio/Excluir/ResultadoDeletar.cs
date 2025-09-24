using System.Runtime.CompilerServices;

namespace Snebur.AcessoDados;

public class ResultadoDeletar : Resultado
{
    #region Campos Privados

    #endregion

    public static ResultadoDeletar CreateErro(
       Exception erro,
       [CallerMemberName] string nomeMetodo = "",
       [CallerFilePath] string caminhoArquivo = "",
       [CallerLineNumber] int linhaDoErro = 0)
    {
        var resultado = new ResultadoDeletar
        {
            IsSucesso = false,
            MensagemErro = ErroUtil.RetornarDescricaoDetalhadaErro(erro),
            Erro = ErroAcessoDados.Create(erro, caminhoArquivo, nomeMetodo, linhaDoErro)
        };
        return resultado;
    }
}