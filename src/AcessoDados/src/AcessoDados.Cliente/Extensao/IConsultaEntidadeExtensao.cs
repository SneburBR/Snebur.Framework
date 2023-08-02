using Snebur.Dominio;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Snebur.AcessoDados
{
    public static class IConsultaEntidadeExtensao
    {
        public static Task<TEntidade> FindAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta, long id) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew(() => consulta.Find(id));
        }

        public static Task<bool> ExistsAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta, long id) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew(() => consulta.Exists(id));
        }

        public static Task<List<TEntidade>> ToListAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew(() => consulta.ToList());
        }

        public static Task<TEntidade> SingleAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew(() => consulta.Single());
        }

        public static Task<TEntidade> SingleOrDefaultAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew(() => consulta.SingleOrDefault());
        }

        public static Task<TEntidade> FirstAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew(() => consulta.First());
        }

        public static Task<TEntidade> FirstOrDefault<TEntidade>(this IConsultaEntidade<TEntidade> consulta) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew(() => consulta.FirstOrDefault());
        }

        #region Sum 

        public static Task<int> Sum<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, int>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew(() => consulta.Sum(propriedade));
        }

        public static Task<int?> Sum<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, int?>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew(() => consulta.Sum(propriedade));
        }

        public static Task<long> SumAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, long>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew(() => consulta.Sum(propriedade));
        }

        public static Task<decimal> SumAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, decimal>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew(() => consulta.Sum(propriedade));
        }

        public static Task<decimal?> SumAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, decimal?>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew(() => consulta.Sum(propriedade));
        }

        public static Task<double> SumAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, double>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<double>(() =>
            {
                return consulta.Sum(propriedade);
            });
        }

        public static Task<double?> SumAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, double?>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew(() => consulta.Sum(propriedade));
        }

        #endregion

        #region Max

        public static Task<int> MaxAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, int>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<int>(() =>
            {
                return consulta.Max(propriedade);
            });
        }

        public static Task<int?> MaxAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, int?>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<int?>(() =>
            {
                return consulta.Max(propriedade);
            });
        }

        public static Task<long> MaxAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, long>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<long>(() =>
            {
                return consulta.Max(propriedade);
            });
        }

        public static Task<decimal> MaxAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, decimal>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<decimal>(() =>
            {
                return consulta.Max(propriedade);
            });
        }

        public static Task<decimal?> MaxAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, decimal?>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<decimal?>(() =>
            {
                return consulta.Max(propriedade);
            });
        }

        public static Task<double> MaxAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, double>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<double>(() =>
            {
                return consulta.Max(propriedade);
            });
        }

        public static Task<double?> MaxAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, double?>> propriedade) where TEntidade : IEntidade
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

        public static Task<int?> MinAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, int?>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<int?>(() =>
            {
                return consulta.Min(propriedade);
            });
        }

        public static Task<long> MinAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, long>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<long>(() =>
            {
                return consulta.Min(propriedade);
            });
        }

        public static Task<decimal> MinAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, decimal>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew(() =>
            {
                return consulta.Min(propriedade);
            });
        }

        public static Task<decimal?> MinAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, decimal?>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<decimal?>(() =>
            {
                return consulta.Min(propriedade);
            });
        }

        public static Task<double> MinAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, double>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<double>(() =>
            {
                return consulta.Min(propriedade);
            });
        }

        public static Task<double?> MinAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, double?>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<double?>(() =>
            {
                return consulta.Min(propriedade);
            });
        }

        #endregion

        #region Average

        public static Task<int> AverageAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, int>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<int>(() =>
            {
                return consulta.Average(propriedade);
            });
        }

        public static Task<int?> AverageAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, int?>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<int?>(() =>
            {
                return consulta.Average(propriedade);
            });
        }

        public static Task<long> AverageAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, long>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<long>(() =>
            {
                return consulta.Average(propriedade);
            });
        }

        public static Task<decimal> AverageAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, decimal>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<decimal>(() =>
            {
                return consulta.Average(propriedade);
            });
        }

        public static Task<decimal?> AverageAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, decimal?>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<decimal?>(() =>
            {
                return consulta.Average(propriedade);
            });
        }

        public static Task<double> AverageAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, double>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<double>(() =>
            {
                return consulta.Average(propriedade);
            });
        }

        public static Task<double?> AverageAsync<TEntidade>(this IConsultaEntidade<TEntidade> consulta, Expression<Func<TEntidade, double?>> propriedade) where TEntidade : IEntidade
        {
            return Task.Factory.StartNew<double?>(() =>
            {
                return consulta.Average(propriedade);
            });
        }

        #endregion
    }

}
