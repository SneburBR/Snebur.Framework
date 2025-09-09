namespace Snebur.AcessoDados.Seguranca;

internal abstract class AutorizacaoEntidade
{
    internal EnumPermissao Permissao { get; private set; }

    internal string NomeTipoEntidade { get; }

    internal EnumOperacao Operacao { get; }

    internal List<AutorizacaoPermissaoEntidade> EstruturasPermissaoEntidade { get; } = new List<AutorizacaoPermissaoEntidade>();

    internal List<IRegraOperacao> RegrasOperacao { get; } = [];

    //internal Dictionary<string, IRegraOperacao>? RegrasOperacaoCampo { get; }

    public bool IsSalvarLogAlteracao { get; }

    public bool IsSalvarLogSeguranca { get; }

    //internal IRegraOperacao RegraOperacao { get; set; }

    //internal Dictionary<string, IPermisaoCampo> Campos { get; set; } = new Dictionary<string, IPermisaoCampo>();

    //internal ListaEntidades<IRestricaoFiltro> RestricoesFiltro { get; set; } = new ListaEntidades<IRestricaoFiltro>();

    internal AutorizacaoEntidade(
        string nomeTipoEntidade,
        EnumOperacao operacao)
    {
        this.NomeTipoEntidade = nomeTipoEntidade;
        this.Operacao = operacao;
        this.Permissao = EnumPermissao.Negado;
    }

    internal void Autorizar(EstruturaPermissaoEntidade estruturaPermissaoEntidade, IRegraOperacao regraOperacao, bool avalistaRequerido)
    {
        var permissao = (avalistaRequerido) ? EnumPermissao.AvalistaRequerido : EnumPermissao.Autorizado;

        this.Permissao = EnumPermissao.Autorizado;
        this.EstruturasPermissaoEntidade.Add(new AutorizacaoPermissaoEntidade(estruturaPermissaoEntidade, permissao));
        this.RegrasOperacao.Add(regraOperacao);

        if (this.Permissao != EnumPermissao.Autorizado)
        {
            this.Permissao = permissao;
        }
    }

    internal List<IPermissaoCampo> RetornarPermissoesCampo()
    {
        throw new NotImplementedException();
    }

    internal List<EstruturaRestricaoFiltro> RetornarRestricoesFiltro()
    {
        return this.EstruturasPermissaoEntidade.SelectMany(x => x.EstruturaPermissaoEntidade.RestricoesFiltro.Values).ToList();
    }
}