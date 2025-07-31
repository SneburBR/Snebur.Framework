using Snebur.Dominio.Atributos;

namespace Snebur.Dominio;

[IgnorarInterfaceTS]
public interface IPermissaoEntidade : IEntidadeSeguranca
{
    [ValidacaoRequerido]
    IIdentificacao? Identificacao { get; set; }

    [PropriedadeDescricao]
    [Indexar]
    [ValidacaoRequerido]
    [ValidacaoTextoTamanho(100)]
    string NomeTipoEntidadePermissao { get; set; }

    //int MaximoRegistroConsulta { get; set; }

    [ValidacaoRequerido]
    IRegraOperacao? Leitura { get; set; }

    [ValidacaoRequerido]
    IRegraOperacao? Adicionar { get; set; }

    [ValidacaoRequerido]
    IRegraOperacao? Atualizar { get; set; }

    [ValidacaoRequerido]
    IRegraOperacao? Deletar { get; set; }

    IEnumerable<IPermissaoCampo> PermissoesCampo { get; }

    IEnumerable<IRestricaoEntidade> RestricoesEntidade { get; }
}