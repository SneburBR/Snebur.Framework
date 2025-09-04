namespace Snebur.AcessoDados;

[IgnorarInterfaceTS]
public interface IConsultaEntidade
{
    int Count();
    bool Any();
    EstruturaConsulta RetornarEstruturaConsulta();

    //IConsultaEntidade<TEntidade> AsConsulta<TEntidade>() where TEntidade : Entidade;
}

[IgnorarInterfaceTS]
public interface IConsultaEntidade<TEntidade> : IConsultaEntidade where TEntidade : IEntidade
{
    ConsultaEntidade<TEspecializacao> OfType<TEspecializacao>() where TEspecializacao : TEntidade;

    ConsultaEntidade<TEntidade> Take(int take);

    ConsultaEntidade<TEntidade> Skip(int skip);

    List<TEntidade> ToList();

    TEntidade Find(long id);

    TEntidade Single();

    TEntidade? SingleOrDefault();

    TEntidade First();

    TEntidade? FirstOrDefault();

    bool Exists(long id);

    ConsultaEntidade<TEntidade> Where(Expression<Func<TEntidade, bool>> filtro);

    ConsultaEntidade<TEntidade> WhereOr(Expression<Func<TEntidade, bool>> filtro);

    ConsultaEntidade<TEntidade> WhereIds(List<long> ids);

    ConsultaEntidade<TEntidade> WhereIn(Expression<Func<TEntidade, string>> expressaoPropriedade, IEnumerable<string> lista);

    ConsultaEntidade<TEntidade> WhereIn(Expression<Func<TEntidade, long>> expressaoPropriedade, IEnumerable<long> lista);

    ConsultaEntidade<TEntidade> WhereIn(Expression<Func<TEntidade, long?>> expressaoPropriedade, IEnumerable<long> lista);

    ConsultaEntidade<TEntidade> WhereIn(Expression<Func<TEntidade, Enum>> expressaoPropriedade, IEnumerable<int> lista);

    ConsultaEntidade<TEntidade> OrderBy(Expression<Func<TEntidade, object?>> expressaoPropriedade);

    ConsultaEntidade<TEntidade> OrderByDescending(Expression<Func<TEntidade, object?>> expressaoPropriedade);

    ConsultaEntidade<TEntidade> AbrirRelacao<TRelacao>(Expression<Func<TEntidade, TRelacao?>> expressao) where TRelacao : IEntidade;

    ConsultaEntidade<TEntidade> AbrirRelacao(List<PropertyInfo> propriedades);

    ConsultaEntidade<TEntidade> AbrirRelacao(string caminhoPropriedade);

    ConsultaEntidade<TEntidade> AbrirRelacoes(params Expression<Func<TEntidade, object>>[] expressoes);

    ConsultaEntidade<TEntidade> AbrirColecao(Expression<Func<TEntidade, IEnumerable>> expressao);

    ConsultaEntidade<TEntidade> AbrirRelacoes(params string[] caminhosPropriedade);

    ConsultaEntidade<TEntidade> AbrirRelacoes(params Expression[] expressoes);

    ConsultaEntidade<TEntidade> AbrirColecao(string caminhoColecao);

    ConsultaEntidade<TEntidade> AbrirColecao<TRelacao>(Expression<Func<TEntidade, IEnumerable<TRelacao>>> expresssao) where TRelacao : IEntidade;

    ConsultaEntidade<TEntidade> AbrirColecoes(params Expression<Func<TEntidade, IEnumerable>>[] expressoes);

    ConsultaEntidade<TEntidade> AbrirPropriedade<TPropriedade>(Expression<Func<TEntidade, TPropriedade>> expresssao);

    ConsultaEntidade<TEntidade> AbrirPropriedade(Expression<Func<TEntidade, string>> expresssao);

    ConsultaEntidade<TEntidade> AbrirPropriedades<TPropriedade>(params Expression<Func<TEntidade, TPropriedade>>[] expresssao);

    ConsultaEntidade<TEntidade> AbrirPropriedades(params Expression<Func<TEntidade, string>>[] expresssao);

    ConsultaEntidade<TEntidade> AbrirPropriedadeTipoComplexo<TTipoComplexo>(Expression<Func<TEntidade, TTipoComplexo>> expresssao) where TTipoComplexo : BaseTipoComplexo;

    ConsultaEntidade<TEntidade> AdicionarFiltroPropriedade(PropertyInfo propriedade, EnumOperadorFiltro operador, object valor);

    //TEntidade RetornarPorId(long id);

    #region Funções

    #region Sum 

    int Sum(Expression<Func<TEntidade, int>> propriedade);

    int? Sum(Expression<Func<TEntidade, int?>> propriedade);

    long Sum(Expression<Func<TEntidade, long>> propriedade);

    decimal Sum(Expression<Func<TEntidade, decimal>> propriedade);

    decimal? Sum(Expression<Func<TEntidade, decimal?>> propriedade);

    double Sum(Expression<Func<TEntidade, double>> propriedade);

    double? Sum(Expression<Func<TEntidade, double?>> propriedade);

    #endregion

    #region Max

    int Max(Expression<Func<TEntidade, int>> propriedade);

    int? Max(Expression<Func<TEntidade, int?>> propriedade);

    long Max(Expression<Func<TEntidade, long>> propriedade);

    decimal Max(Expression<Func<TEntidade, decimal>> propriedade);

    decimal? Max(Expression<Func<TEntidade, decimal?>> propriedade);

    double Max(Expression<Func<TEntidade, double>> propriedade);

    double? Max(Expression<Func<TEntidade, double?>> propriedade);

    DateTime Max(Expression<Func<TEntidade, DateTime>> propriedade);

    DateTime? Max(Expression<Func<TEntidade, DateTime?>> propriedade);

    #endregion

    #region Min

    int Min(Expression<Func<TEntidade, int>> propriedade);

    int? Min(Expression<Func<TEntidade, int?>> propriedade);

    long Min(Expression<Func<TEntidade, long>> propriedade);

    decimal Min(Expression<Func<TEntidade, decimal>> propriedade);

    decimal? Min(Expression<Func<TEntidade, decimal?>> propriedade);

    double Min(Expression<Func<TEntidade, double>> propriedade);

    double? Min(Expression<Func<TEntidade, double?>> propriedade);

    DateTime Min(Expression<Func<TEntidade, DateTime>> propriedade);

    DateTime? Min(Expression<Func<TEntidade, DateTime?>> propriedade);

    #endregion

    #region Average

    int Average(Expression<Func<TEntidade, int>> propriedade);

    int? Average(Expression<Func<TEntidade, int?>> propriedade);

    long Average(Expression<Func<TEntidade, long>> propriedade);

    decimal Average(Expression<Func<TEntidade, decimal>> propriedade);

    decimal? Average(Expression<Func<TEntidade, decimal?>> propriedade);

    double Average(Expression<Func<TEntidade, double>> propriedade);

    double? Average(Expression<Func<TEntidade, double?>> propriedade);

    DateTime Average(Expression<Func<TEntidade, DateTime>> propriedade);

    DateTime? Average(Expression<Func<TEntidade, DateTime?>> propriedade);

    #endregion

    #endregion

    #region Coleção

    ConsultaEntidade<TEntidade> WhereColecao<TRelacao>(Expression<Func<TEntidade, ListaEntidades<TRelacao>>> caminhoColecao, Expression<Func<TRelacao, bool>> filtro) where TRelacao : Entidade;

    ConsultaEntidade<TEntidade> OrderByColecao<TRelacao>(Expression<Func<TEntidade, ListaEntidades<TRelacao>>> caminhoColecao, Expression<Func<TRelacao, object>> expressaoCaminhoPropriedade) where TRelacao : Entidade;

    ConsultaEntidade<TEntidade> OrderByDescendingColecao<TRelacao>(Expression<Func<TEntidade, ListaEntidades<TRelacao>>> caminhoColecao, Expression<Func<TRelacao, object>> expressaoCaminhoPropriedade) where TRelacao : Entidade;

    ConsultaEntidade<TEntidade> TakeColecao<TRelacao>(Expression<Func<TEntidade, ListaEntidades<TRelacao>>> caminhoColecao, int take) where TRelacao : Entidade;

    ConsultaEntidade<TEntidade> SkipColecao<TRelacao>(Expression<Func<TEntidade, ListaEntidades<TRelacao>>> caminhoColecao, int skip) where TRelacao : Entidade;

    #endregion

    ConsultaEntidade<TEntidade> IncluirDeletados();

}