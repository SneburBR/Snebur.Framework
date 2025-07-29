using Snebur.Dominio;
using System;
using System.Linq.Expressions;

namespace Snebur.AcessoDados;


public partial class ConsultaEntidade<TEntidade> where TEntidade : IEntidade
{

    public int Max(Expression<Func<TEntidade, int>> propriedade)
    {
        return this.RetonrarValorMax<int>(propriedade);
    }

    public int? Max(Expression<Func<TEntidade, int?>> propriedade)
    {
        return this.RetonrarValorMax<int?>(propriedade);
    }

    public long Max(Expression<Func<TEntidade, long>> propriedade)
    {
        return this.RetonrarValorMax<long>(propriedade);
    }

    public long? Max(Expression<Func<TEntidade, long?>> propriedade)
    {
        return this.RetonrarValorMax<long?>(propriedade);
    }

    public decimal Max(Expression<Func<TEntidade, decimal>> propriedade)
    {
        return this.RetonrarValorMax<decimal>(propriedade);
    }

    public decimal? Max(Expression<Func<TEntidade, decimal?>> propriedade)
    {
        return this.RetonrarValorMax<decimal?>(propriedade);
    }

    public double Max(Expression<Func<TEntidade, double>> propriedade)
    {
        return this.RetonrarValorMax<double>(propriedade);
    }

    public double? Max(Expression<Func<TEntidade, double?>> propriedade)
    {
        return this.RetonrarValorMax<double?>(propriedade);
    }

    public DateTime Max(Expression<Func<TEntidade, DateTime>> propriedade)
    {
        return this.RetonrarValorMax<DateTime>(propriedade);
    }

    public DateTime? Max(Expression<Func<TEntidade, DateTime?>> propriedade)
    {
        return this.RetonrarValorMax<DateTime?>(propriedade);
    }

    private T RetonrarValorMax<T>(Expression expressao)
    {
        return this.RetornarValorFuncao<T>(EnumTipoFuncao.Maximo, expressao);
    }
}
