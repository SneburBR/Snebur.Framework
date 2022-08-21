using Snebur.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Snebur.AcessoDados

{
    public partial class ConsultaEntidade<TEntidade> where TEntidade : IEntidade
    {

        public int Sum(Expression<Func<TEntidade, int>> propriedade)
        {
            return this.RetonrarValorSum<int>(propriedade);
        }

        public int? Sum(Expression<Func<TEntidade, int?>> propriedade)
        {
            return this.RetonrarValorSum<int?>(propriedade);
        }

        public long Sum(Expression<Func<TEntidade, long>> propriedade)
        {
            return this.RetonrarValorSum<long>(propriedade);
        }

        public long? Sum(Expression<Func<TEntidade, long?>> propriedade)
        {
            return this.RetonrarValorSum<long?>(propriedade);
        }

        public decimal Sum(Expression<Func<TEntidade, decimal>> propriedade)
        {
            return this.RetonrarValorSum<decimal>(propriedade);
        }

        public decimal? Sum(Expression<Func<TEntidade, decimal?>> propriedade)
        {
            return this.RetonrarValorSum<decimal?>(propriedade);
        }

        public double Sum(Expression<Func<TEntidade, double>> propriedade)
        {
            return this.RetonrarValorSum<double>(propriedade);
        }

        public double? Sum(Expression<Func<TEntidade, double?>> propriedade)
        {
            return this.RetonrarValorSum<double?>(propriedade);
        }
        //public DateTime Sum(Expression<Func<TEntidade, DateTime>> propriedade)
        //{
        //    return this.RetonrarValorSum<DateTime>(propriedade);
        //}

        //public DateTime? Sum(Expression<Func<TEntidade, DateTime?>> propriedade)
        //{
        //    return this.RetonrarValorSum<DateTime?>(propriedade);
        //}

        private T RetonrarValorSum<T>(Expression expressao)
        {
            return this.RetornarValorFuncao<T>(EnumTipoFuncao.Somar, expressao);
        }
    }
}