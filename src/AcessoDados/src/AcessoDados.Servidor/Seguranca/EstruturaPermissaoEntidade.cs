namespace Snebur.AcessoDados.Seguranca;

internal class EstruturaPermissaoEntidade
{
    //internal IIdentificacao? Identificacao { get; }

    internal IPermissaoEntidade PermissaoEntidade { get; }

    internal Dictionary<string, EstruturaPermissaoCampo> PermissoesCampo { get; } = new();

    internal Dictionary<string, EstruturaRestricaoFiltro> RestricoesFiltro { get; } = new();

    internal IRegraOperacao RetornarRegraOperacao(EnumOperacao operacao)
    {
        return RetornarRegraOperacaoInterna(operacao) ?? throw new Exception($"A operação não está configurada {EnumUtil.RetornarDescricao(operacao)}");
    }

    internal IRegraOperacao? RetornarRegraOperacaoInterna(EnumOperacao operacao)
    {
        switch (operacao)
        {
            case EnumOperacao.Leitura:

                return this.PermissaoEntidade.Leitura;

            case EnumOperacao.Adicionar:

                return this.PermissaoEntidade.Adicionar;

            case EnumOperacao.Atualizar:

                return this.PermissaoEntidade.Atualizar;

            case EnumOperacao.Deletar:

                return this.PermissaoEntidade.Deletar;

            default:

                throw new Erro(String.Format("A operação não é suportada {0}", EnumUtil.RetornarDescricao(operacao)));
        }
    }

    internal EstruturaPermissaoEntidade(IPermissaoEntidade permissaoEntidade)
    {
        Guard.NotNull(permissaoEntidade);
        Guard.NotNull(permissaoEntidade.Leitura);
        Guard.NotNull(permissaoEntidade.Atualizar);
        Guard.NotNull(permissaoEntidade.Adicionar);
        Guard.NotNull(permissaoEntidade.Deletar);

        this.PermissaoEntidade = permissaoEntidade;

        foreach (var permissaoCampo in this.PermissaoEntidade.PermissoesCampo)
        {
            this.PermissoesCampo.Add(permissaoCampo.NomeCampo, new EstruturaPermissaoCampo(permissaoCampo));
        }

        foreach (var restricao in this.PermissaoEntidade.RestricoesEntidade)
        {
            throw new NotImplementedException();
        }
    }
}