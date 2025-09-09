namespace Snebur.AcessoDados;


public partial class ConsultaEntidade<TEntidade> where TEntidade : IEntidade
{

    public int Average(Expression<Func<TEntidade, int>> propriedade)
    {
        return this.RetonrarValorAverage<int>(propriedade);
    }

    public int? Average(Expression<Func<TEntidade, int?>> propriedade)
    {
        return this.RetonrarValorAverage<int?>(propriedade);
    }

    public long Average(Expression<Func<TEntidade, long>> propriedade)
    {
        return this.RetonrarValorAverage<long>(propriedade);
    }

    public long? Average(Expression<Func<TEntidade, long?>> propriedade)
    {
        return this.RetonrarValorAverage<long?>(propriedade);
    }

    public decimal Average(Expression<Func<TEntidade, decimal>> propriedade)
    {
        return this.RetonrarValorAverage<decimal>(propriedade);
    }

    public decimal? Average(Expression<Func<TEntidade, decimal?>> propriedade)
    {
        return this.RetonrarValorAverage<decimal?>(propriedade);
    }

    public double Average(Expression<Func<TEntidade, double>> propriedade)
    {
        return this.RetonrarValorAverage<double>(propriedade);
    }

    public double? Average(Expression<Func<TEntidade, double?>> propriedade)
    {
        return this.RetonrarValorAverage<double?>(propriedade);
    }

    public DateTime Average(Expression<Func<TEntidade, DateTime>> propriedade)
    {
        return this.RetonrarValorAverage<DateTime>(propriedade);
    }

    public DateTime? Average(Expression<Func<TEntidade, DateTime?>> propriedade)
    {
        return this.RetonrarValorAverage<DateTime?>(propriedade);
    }

    private T? RetonrarValorAverage<T>(Expression expressao)
    {
        return this.RetornarValorFuncao<T>(EnumTipoFuncao.Media, expressao);
    }
}