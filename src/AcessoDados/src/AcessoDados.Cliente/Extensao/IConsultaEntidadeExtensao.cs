using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Snebur.Dominio;

namespace Snebur.AcessoDados
{
    public static class IConsultaEntidadeExtensao
    {
        public static Task<ListaEntidades<TEntidade>> ToListAwait<TEntidade>(this IConsultaEntidade<TEntidade> consulta) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<ListaEntidades<TEntidade>>(() =>
            {
                return consulta.ToList();
            });
        }

        public static Task<TEntidade> SingleAwait<TEntidade>(this IConsultaEntidade<TEntidade> consulta) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<TEntidade>(() =>
            {
                return consulta.Single();
            });
        }

        public static Task<TEntidade> SingleOrDefaultAwait<TEntidade>(this IConsultaEntidade<TEntidade> consulta) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<TEntidade>(() =>
            {
                return consulta.SingleOrDefault();
            });
        }

        public static Task<TEntidade> FirstAwait<TEntidade>(this IConsultaEntidade<TEntidade> consulta) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<TEntidade>(() =>
            {
                return consulta.First();
            });
        }

        public static Task<TEntidade> FirstOrDefault<TEntidade>(this IConsultaEntidade<TEntidade> consulta) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<TEntidade>(() =>
            {
                return consulta.FirstOrDefault();
            });
        }

        #region Sum 

        public static Task<int> Sum<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, int>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<int>(() =>
            {
                return consulta.Sum(propriedade);
            });
        }

        public static Task<int?> Sum<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, int?>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<int?>(() =>
            {
                return consulta.Sum(propriedade);
            });
        }

        public static Task<long> Sum<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, long>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<long>(() =>
            {
                return consulta.Sum(propriedade);
            });
        }

        public static Task<decimal> Sum<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, decimal>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<decimal>(() =>
            {
                return consulta.Sum(propriedade);
            });
        }

        public static Task<decimal?> Sum<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, decimal?>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<decimal?>(() =>
            {
                return consulta.Sum(propriedade);
            });
        }

        public static Task<double> Sum<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, double>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<double>(() =>
            {
                return consulta.Sum(propriedade);
            });
        }

        public static Task<double?> Sum<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, double?>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<double?>(() =>
            {
                return consulta.Sum(propriedade);
            });
        }

        #endregion

        #region Max

        public static Task<int> Max<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, int>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<int>(() =>
            {
                return consulta.Max(propriedade);
            });
        }

        public static Task<int?> Max<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, int?>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<int?>(() =>
            {
                return consulta.Max(propriedade);
            });
        }

        public static Task<long> Max<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, long>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<long>(() =>
            {
                return consulta.Max(propriedade);
            });
        }

        public static Task<decimal> Max<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, decimal>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<decimal>(() =>
            {
                return consulta.Max(propriedade);
            });
        }

        public static Task<decimal?> Max<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, decimal?>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<decimal?>(() =>
            {
                return consulta.Max(propriedade);
            });
        }

        public static Task<double> Max<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, double>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<double>(() =>
            {
                return consulta.Max(propriedade);
            });
        }

        public static Task<double?> Max<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, double?>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<double?>(() =>
            {
                return consulta.Max(propriedade);
            });
        }

        #endregion

        #region Min


        public static Task<int> Min<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, int>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<int>(() =>
            {
                return consulta.Min(propriedade);
            });
        }

        public static Task<int?> Min<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, int?>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<int?>(() =>
            {
                return consulta.Min(propriedade);
            });
        }

        public static Task<long> Min<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, long>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<long>(() =>
            {
                return consulta.Min(propriedade);
            });
        }

        public static Task<decimal> Min<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, decimal>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<decimal>(() =>
            {
                return consulta.Min(propriedade);
            });
        }

        public static Task<decimal?> Min<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, decimal?>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<decimal?>(() =>
            {
                return consulta.Min(propriedade);
            });
        }

        public static Task<double> Min<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, double>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<double>(() =>
            {
                return consulta.Min(propriedade);
            });
        }

        public static Task<double?> Min<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, double?>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<double?>(() =>
            {
                return consulta.Min(propriedade);
            });
        }

        #endregion

        #region Average

        public static Task<int> Average<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, int>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<int>(() =>
            {
                return consulta.Average(propriedade);
            });
        }

        public static Task<int?> Average<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, int?>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<int?>(() =>
            {
                return consulta.Average(propriedade);
            });
        }

        public static Task<long> Average<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, long>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<long>(() =>
            {
                return consulta.Average(propriedade);
            });
        }

        public static Task<decimal> Average<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, decimal>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<decimal>(() =>
            {
                return consulta.Average(propriedade);
            });
        }

        public static Task<decimal?> Average<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, decimal?>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<decimal?>(() =>
            {
                return consulta.Average(propriedade);
            });
        }

        public static Task<double> Average<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, double>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<double>(() =>
            {
                return consulta.Average(propriedade);
            });
        }

        public static Task<double?> Average<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, double?>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<double?>(() =>
            {
                return consulta.Average(propriedade);
            });
        }

        #endregion
    }

}
