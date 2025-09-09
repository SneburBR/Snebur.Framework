using Snebur.AcessoDados.Ajudantes;

namespace Snebur.AcessoDados;

public partial class ConsultaEntidade<TEntidade> where TEntidade : IEntidade
{
    //public ConsultaEntidade<TEntidade> AbrirRelacao<TRelacao>(Expression<Func<TEntidade, IEnumerable<TRelacao>>> expressao) where TRelacao : IEntidade
    //{
    //    return this.AbrirColecao(expressao);
    //}

    public ConsultaEntidade<TEntidade> AbrirColecao(string caminhoColecao)
    {
        return this.AbrirRelacao(this.EstruturaConsulta, caminhoColecao, false);
    }

    public ConsultaEntidade<TEntidade> AbrirColecao<TRelacao>(Expression<Func<TEntidade, IEnumerable<TRelacao>>> expressao) where TRelacao : IEntidade
    {
        var expressaoInterna = (Expression)expressao;
        return this.AbrirRelacao(this.EstruturaConsulta, expressaoInterna, false);
    }

    public ConsultaEntidade<TEntidade> AbrirColecao(Expression<Func<TEntidade, IEnumerable>> expressao)
    {
        var expressaoInterna = (Expression)expressao;
        return this.AbrirRelacao(this.EstruturaConsulta, expressaoInterna, false);
    }

    public ConsultaEntidade<TEntidade> AbrirColecoes(params string[] caminhosColecao)
    {
        foreach (var caminhoColecao in caminhosColecao)
        {
            return this.AbrirRelacao(caminhoColecao);
        }
        return this;
    }

    public ConsultaEntidade<TEntidade> AbrirColecoes(params Expression<Func<TEntidade, IEnumerable>>[] expressoes)
    {
        foreach (var expessao in expressoes)
        {
            this.AbrirColecao(expessao);
        }
        return this;
    }
    //public ConsultaEntidade<TEntidade> AbrirColecoes<TRelacao>(params Expression<Func<TEntidade, IEnumerable<TRelacao>>>[] expressoes) where TRelacao : IEntidade
    //{
    //    foreach(var expressao in expressoes)
    //    {
    //        this.AbrirColecao(expressao);
    //    }
    //    return this;
    //}

    public ConsultaEntidade<TEntidade> WhereColecao<TRelacao>(Expression<Func<TEntidade, ListaEntidades<TRelacao>>> expressaoCaminhoColecao, Expression<Func<TRelacao, bool>> filtro) where TRelacao : Entidade
    {
        var consultaAcessoDados = this.RetornarEstruturaConsultaRelacaoColecao(expressaoCaminhoColecao);
        this.AdicionarFiltro(consultaAcessoDados, filtro, consultaAcessoDados.FiltroGrupoE);
        return this;
    }

    public ConsultaEntidade<TEntidade> OrderByColecao<TRelacao>(Expression<Func<TEntidade, ListaEntidades<TRelacao>>> expressaoCaminhoColecao, Expression<Func<TRelacao, object>> expressaoCaminhoPropriedade) where TRelacao : Entidade
    {
        var consultaAcessoDados = this.RetornarEstruturaConsultaRelacaoColecao(expressaoCaminhoColecao);
        this.Ordernar(consultaAcessoDados, expressaoCaminhoPropriedade, EnumSentidoOrdenacao.Crescente);
        return this;
    }

    public ConsultaEntidade<TEntidade> OrderByDescendingColecao<TRelacao>(Expression<Func<TEntidade, ListaEntidades<TRelacao>>> expressaoCaminhoColecao, Expression<Func<TRelacao, object>> expressaoCaminhoPropriedade) where TRelacao : Entidade
    {
        var consultaAcessoDados = this.RetornarEstruturaConsultaRelacaoColecao(expressaoCaminhoColecao);
        this.Ordernar(consultaAcessoDados, expressaoCaminhoPropriedade, EnumSentidoOrdenacao.Decrescente);
        return this;
    }

    public ConsultaEntidade<TEntidade> TakeColecao<TRelacao>(Expression<Func<TEntidade, ListaEntidades<TRelacao>>> expressaoCaminhoColecao, int take) where TRelacao : Entidade
    {
        var consultaAcessoDados = this.RetornarEstruturaConsultaRelacaoColecao(expressaoCaminhoColecao);
        consultaAcessoDados.Take = take;
        return this;
    }

    public ConsultaEntidade<TEntidade> SkipColecao<TRelacao>(Expression<Func<TEntidade, ListaEntidades<TRelacao>>> expressaoCaminhoColecao, int skip) where TRelacao : Entidade
    {
        var consultaAcessoDados = this.RetornarEstruturaConsultaRelacaoColecao(expressaoCaminhoColecao);
        consultaAcessoDados.Skip = skip;
        return this;
    }

    private EstruturaConsulta RetornarEstruturaConsultaRelacaoColecao(Expression expressaoCaminhoColecao)
    {
        var propriedades = ExpressaoUtil.RetornarPropriedades(expressaoCaminhoColecao);
        var propriedadesCaminho = new List<PropertyInfo>();

        EstruturaConsulta estruturaConsultaAtual = this.EstruturaConsulta;
        foreach (var propriedade in propriedades)
        {
            propriedadesCaminho.Add(propriedade);
            var caminhoPropriedade = AjudanteConsultaEntidade.RetornarCaminhoPropriedade(propriedadesCaminho);

            Guard.NotNullOrWhiteSpace(caminhoPropriedade);
            //ErroUtil.ValidarStringVazia(caminhoPropriedade, nameof(caminhoPropriedade));

            if (ReflexaoUtil.IsPropriedadeRetornaTipoPrimario(propriedade))
            {
                continue;
            }

            if (propriedade.PropertyType.IsSubclassOf(typeof(Entidade)))
            {
                continue;
            }

            if (AjudanteConsultaEntidade.IsPropriedadeRetornarListaEntidade(propriedade))
            {
                if (!estruturaConsultaAtual.ColecoesAberta.ContainsKey(caminhoPropriedade))
                {
                    throw new Erro(String.Format("A preciso abrir a relação {0} ander de filtra-la, Caminho completo {1}", caminhoPropriedade, expressaoCaminhoColecao));
                }
                var relacaoAbertaColecao = (RelacaoAbertaColecao)estruturaConsultaAtual.ColecoesAberta[caminhoPropriedade];
                estruturaConsultaAtual = relacaoAbertaColecao.EstruturaConsulta!;
                Guard.NotNull(estruturaConsultaAtual);
            }
        }
        return estruturaConsultaAtual;
    }
};