using Snebur.AcessoDados.Seguranca;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Snebur.AcessoDados;

public class ResultadoSalvar : Resultado
{
    #region Campos Privados

    #endregion

    public List<EntidadeSalvaInfo> EntidadesSalvas { get; set; } = new List<EntidadeSalvaInfo>();

    public List<ErroValidacaoInfo> ErrosValidacao { get; set; } = new List<ErroValidacaoInfo>();

    [JsonConstructor]
    private ResultadoSalvar()
    {

    }

    public static ResultadoSalvar CreateErro(
        Exception erro,
        [CallerMemberName] string nomeMetodo = "",
        [CallerFilePath] string caminhoArquivo = "",
        [CallerLineNumber] int linhaDoErro = 0)
    {
        var resultado = new ResultadoSalvar
        {
            IsSucesso = false,
            MensagemErro = ErroUtil.RetornarDescricaoDetalhadaErro(erro),
            Erro = ErroAcessoDados.Create(erro, caminhoArquivo, nomeMetodo, linhaDoErro)
        };
        return resultado;
    }

    public static ResultadoSalvar Sucesso()
    {
        return new ResultadoSalvar
        {
            IsSucesso = true
        };
    }

    public static ResultadoSalvar CreateErroValidacao(List<ErroValidacaoInfo> errosInfo)
    {
        var erro = new ErroValidacao(errosInfo);
        return new ResultadoSalvar
        {
            IsSucesso = false,
            ErrosValidacao = errosInfo,
            MensagemErro = ErroValidacao.RetornarMensagemErro(errosInfo),
            Erro = new ErroValidacao(errosInfo)

        };
    }

    public static ResultadoSalvar CreateErroPermisao(EnumPermissao permissao)
    {
        var erroPermisao = new ErroPermissao(permissao, $"Permissão {EnumUtil.RetornarDescricao(permissao)} ");
        return new ResultadoSalvar
        {
            IsSucesso = false,
            MensagemErro = erroPermisao.Message,
            Erro = erroPermisao,
            Permissao = permissao
        };
    }
}