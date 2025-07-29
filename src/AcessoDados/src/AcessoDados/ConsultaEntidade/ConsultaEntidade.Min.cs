using Snebur.Dominio;
using System;
using System.Linq.Expressions;

namespace Snebur.AcessoDados;


public partial class ConsultaEntidade<TEntidade> where TEntidade : IEntidade
{

    public int Min(Expression<Func<TEntidade, int>> propriedade)
    {
        return this.RetonrarValorMin<int>(propriedade);
    }

    public int? Min(Expression<Func<TEntidade, int?>> propriedade)
    {
        return this.RetonrarValorMin<int?>(propriedade);
    }

    public long Min(Expression<Func<TEntidade, long>> propriedade)
    {
        return this.RetonrarValorMin<long>(propriedade);
    }

    public long? Min(Expression<Func<TEntidade, long?>> propriedade)
    {
        return this.RetonrarValorMin<long?>(propriedade);
    }

    public decimal Min(Expression<Func<TEntidade, decimal>> propriedade)
    {
        return this.RetonrarValorMin<decimal>(propriedade);
    }

    public decimal? Min(Expression<Func<TEntidade, decimal?>> propriedade)
    {
        return this.RetonrarValorMin<decimal?>(propriedade);
    }

    public double Min(Expression<Func<TEntidade, double>> propriedade)
    {
        return this.RetonrarValorMin<double>(propriedade);
    }

    public double? Min(Expression<Func<TEntidade, double?>> propriedade)
    {
        return this.RetonrarValorMin<double?>(propriedade);
    }

    public DateTime Min(Expression<Func<TEntidade, DateTime>> propriedade)
    {
        return this.RetonrarValorMin<DateTime>(propriedade);
    }

    public DateTime? Min(Expression<Func<TEntidade, DateTime?>> propriedade)
    {
        return this.RetonrarValorMin<DateTime?>(propriedade);
    }

    private T RetonrarValorMin<T>(Expression expressao)
    {
        return this.RetornarValorFuncao<T>(EnumTipoFuncao.Minimo, expressao);
    }
}
