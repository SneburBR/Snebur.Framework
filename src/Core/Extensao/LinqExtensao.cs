using System.Collections;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Snebur.Linq;

public static class LinqExtensao
{
    public static readonly object __lock = new object();

    public static object SyncLock(this IEnumerable enumerable,
        [CallerArgumentExpression(nameof(enumerable))] string? paramName = null)
    {
        Guard.NotNull(enumerable, paramName);

        if (IsConcurrentCollection(enumerable))
        {
            return __lock;
        }
        return (enumerable as ICollection)?.SyncRoot ?? __lock;
    }

    public static T? TryGet<T>(this IList<T> colecao, int index,
        [CallerArgumentExpression(nameof(colecao))] string? paramName = null)
    {
        Guard.NotNull(colecao, paramName);
        if (index < colecao.Count)
        {
            return colecao[index];
        }
        return default;
    }

    public static string? TryGet(this string[] colecao, int index,
        [CallerArgumentExpression(nameof(colecao))] string? paramName = null)
    {
        Guard.NotNull(colecao, paramName);

        if (index < colecao.Length)
        {
            return colecao[index];
        }
        return null;
    }

    public static void AddIsNotNull<T>(this ICollection<T> colecao, T? item,
        [CallerArgumentExpression(nameof(colecao))] string? paramName = null)
    {
        Guard.NotNull(colecao, paramName);
        if (item != null)
        {
            colecao.Add(item);
        }
    }

    public static T? Pop<T>(this IList<T> colecao,
        [CallerArgumentExpression(nameof(colecao))] string? paramName = null)
    {
        Guard.NotNull(colecao, paramName);
        if (colecao.Count > 0)
        {
            lock (colecao.SyncLock())
            {
                var item = colecao[colecao.Count - 1];
                colecao.RemoveAt(colecao.Count - 1);
                return item;
            }
        }
        return default;
    }

    //shift
    public static T? Shift<T>(this IList<T> colecao,
        [CallerArgumentExpression(nameof(colecao))] string? paramName = null)
    {
        Guard.NotNull(colecao, paramName);

        if (colecao.Count > 0)
        {
            lock (colecao.SyncLock())
            {
                var item = colecao[0];
                colecao.RemoveAt(0);
                return item;
            }
        }
        return default;

    }

    public static void AddIfTrue<T>(this ICollection<T> colecao, T item, bool isAdd,
        [CallerArgumentExpression(nameof(colecao))] string? paramName = null)
    {
        Guard.NotNull(colecao, paramName);
        if (isAdd)
        {
            colecao.Add(item);
        }
    }
    public static void AddIfNotExits<T>(this ICollection<T> colecao, T item,
        [CallerArgumentExpression(nameof(colecao))] string? paramName = null)
    {
        Guard.NotNull(colecao, paramName);
        if (!colecao.Contains(item))
        {
            colecao.Add(item);
        }
    }

    public static void AddRangeIfNotExits<T>(
        this ICollection<T> colecao,
        IEnumerable<T> itens,
        [CallerArgumentExpression(nameof(colecao))] string? paramName = null)
    {
        Guard.NotNull(colecao, paramName);

        if (itens == null)
        {
            return;
        }

        foreach (var item in itens)
        {
            colecao.AddIfNotExits(item);
        }
    }

    public static void AddRangeNew<T>(this ICollection<T> colecao,
        IEnumerable<T> itens,
        [CallerArgumentExpression(nameof(colecao))] string? paramName = null)
    {
        Guard.NotNull(paramName);

        if (itens == null)
        {
            return;
        }

        lock (colecao.SyncLock())
        {
            colecao.Clear();
            foreach (var item in itens)
            {
                colecao.Add(item);
            }
        }
    }

    public static void AddRange<T>(
        this ICollection<T> colecao,
        IEnumerable<T>? itens,
        [CallerArgumentExpression(nameof(colecao))] string? paramName = null)
    {
        Guard.NotNull(colecao, paramName);

        if (itens is null)
        {
            return;
        }

        foreach (var item in itens)
        {
            colecao.Add(item);
        }
    }

    public static void AddRangeNotNull<T>(
        this ICollection<T> colecao,
        IEnumerable<T?>? itens,
        [CallerArgumentExpression(nameof(colecao))] string? paramName = null)
    {
        Guard.NotNull(paramName);

        if (itens == null)
        {
            return;
        }

        foreach (var item in itens)
        {
            colecao.AddIsNotNull(item);
        }
    }

    public static void RemoveRange<T>(
        this ICollection<T> colecao,
        IEnumerable<T> itens,
        [CallerArgumentExpression(nameof(colecao))] string? paramName = null)
    {
        Guard.NotNull(colecao, paramName);

        if (itens == null)
        {
            return;
        }

        foreach (var item in itens.ToList())
        {
            colecao.Remove(item);
        }
    }

    public static IEnumerable<T> Duplicados<T>(this IEnumerable<T> item,
        [CallerArgumentExpression(nameof(item))] string? paramName = null)
    {
        Guard.NotNull(item, paramName);
        return item.GroupBy(x => x).
                    SelectMany(g => g.Skip(1)).
                    Distinct();
    }

    public static ListaEntidades<TEntidade> ToList<TEntidade>(
        this ListaEntidades<TEntidade> colecao,
        [CallerArgumentExpression(nameof(colecao))] string? paramName = null) where TEntidade : Entidade
    {
        Guard.NotNull(colecao, paramName);
        var lista = new ListaEntidades<TEntidade>();
        lista.AddRange(colecao);
        return lista;
    }

    public static ListaEntidades<TEntidade> ToListaEntidades<TEntidade>(
        this IEnumerable<TEntidade> colecao,
        [CallerArgumentExpression(nameof(colecao))] string? paramName = null) where TEntidade : IEntidade
    {
        Guard.NotNull(colecao, paramName);
        var listaEntidades = new ListaEntidades<TEntidade>();
        listaEntidades.AddRange(colecao);
        return listaEntidades;
    }
    public static Queue<T> ToQueue<T>(this IEnumerable<T> colecao,
        [CallerArgumentExpression(nameof(colecao))] string? paramName = null)
    {
        Guard.NotNull(colecao, paramName);
        var fila = new Queue<T>();
        foreach (var item in colecao)
        {
            fila.Enqueue(item);
        }
        return fila;
    }

    public static T Random<T>(this IList<T> colecao,
        [CallerArgumentExpression(nameof(colecao))] string? paramName = null)
    {
        Guard.NotNull(colecao, paramName);
        return colecao[new Random().Next(colecao.Count)];
    }

    public static TSource? MinOrDefault<TSource>(this IEnumerable<TSource> colecao,
        [CallerArgumentExpression(nameof(colecao))] string? paramName = null)
    {
        Guard.NotNull(colecao, paramName);
        if (colecao.Count() > 0)
        {
            return colecao.Min();
        }
        return default;
    }

    public static TResult? MinOrDefault<TSource, TResult>(
        this IEnumerable<TSource> colecao, Func<TSource, TResult> selector,
        [CallerArgumentExpression(nameof(colecao))] string? paramName = null)
    {
        Guard.NotNull(colecao, paramName);
        if (colecao.Count() > 0)
        {
            return colecao.Min(selector);
        }
        return default;
    }

    public static TSource? MaxOrDefault<TSource>(this ICollection<TSource> colecao,
        [CallerArgumentExpression(nameof(colecao))] string? paramName = null)
    {
        Guard.NotNull(colecao, paramName);
        if (colecao.Count > 0)
        {
            return colecao.Max();
        }
        return default;
    }

    public static TResult? MaxOrDefault<TSource, TResult>(
        this ICollection<TSource> colecao, Func<TSource, TResult> selector,
        [CallerArgumentExpression(nameof(colecao))] string? paramName = null)
    {
        Guard.NotNull(colecao, paramName);

        if (colecao.Count > 0)
        {
            return colecao.Max(selector);
        }
        return default(TResult);
    }

    public static TSource Random<TSource>(this ICollection<TSource> colecao,
        [CallerArgumentExpression(nameof(colecao))] string? paramName = null)
    {
        Guard.NotNull(colecao, paramName);
        var count = colecao.Count;
        if (count > 0)
        {
            if (count == 1)
            {
                return colecao.First();
            }
            var posicaoRandow = new Random().Next(0, count);
            if (posicaoRandow < count)
            {
                return colecao.ElementAt(posicaoRandow);
            }
            return colecao.First();
        }
        throw new Erro("A coleção não possui nenhum elemento");
    }

    public static TSource? RandomOrDefault<TSource>(this ICollection<TSource> colecao,
        [CallerArgumentExpression(nameof(colecao))] string? paramName = null)
    {
        Guard.NotNull(colecao, paramName);
        var count = colecao.Count;
        if (count > 0)
        {
            return colecao.Random();
        }
        return default;
    }

    #region Sum

    public static float SumOrDefault<TSource>(this IEnumerable<TSource> source,
        Func<TSource, float> selector,
        [CallerArgumentExpression(nameof(source))] string? paramName = null)
    {
        Guard.NotNull(source, paramName);
        if (source.Count() > 0)
        {
            return source.Sum(selector);
        }
        return default;
    }

    public static int SumOrDefault<TSource>(
        this IEnumerable<TSource> source, Func<TSource, int> selector,
        [CallerArgumentExpression(nameof(source))] string? paramName = null)
    {
        Guard.NotNull(source, paramName);
        if (source.Count() > 0)
        {
            return source.Sum(selector);
        }
        return default;
    }

    public static long SumOrDefault<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, long> selector,
        [CallerArgumentExpression(nameof(source))] string? paramName = null)
    {
        Guard.NotNull(source, paramName);
        if (source.Count() > 0)
        {
            return source.Sum(selector);
        }
        return default;
    }

    public static decimal? SumOrDefault<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, decimal?> selector,
        [CallerArgumentExpression(nameof(source))] string? paramName = null)
    {
        Guard.NotNull(source, paramName);
        if (source.Count() > 0)
        {
            return source.Sum(selector);
        }
        return default;
    }

    public static double SumOrDefault<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, double> selector,
        [CallerArgumentExpression(nameof(source))] string? paramName = null)
    {
        Guard.NotNull(source, paramName);
        if (source.Count() > 0)
        {
            return source.Sum(selector);
        }
        return default;
    }

    public static int? SumOrDefault<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, int?> selector,
        [CallerArgumentExpression(nameof(source))] string? paramName = null)
    {
        Guard.NotNull(source, paramName);
        if (source.Count() > 0)
        {
            return source.Sum(selector);
        }
        return default;
    }

    public static long? SumOrDefault<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, long?> selector,
        [CallerArgumentExpression(nameof(source))] string? paramName = null)
    {
        Guard.NotNull(source, paramName);
        if (source.Count() > 0)
        {
            return source.Sum(selector);
        }
        return default;
    }

    public static float? SumOrDefault<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, float?> selector,
        [CallerArgumentExpression(nameof(source))] string? paramName = null)
    {
        Guard.NotNull(source, paramName);
        if (source.Count() > 0)
        {
            return source.Sum(selector);
        }
        return default;
    }

    public static double? SumOrDefault<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, double?> selector,
        [CallerArgumentExpression(nameof(source))] string? paramName = null)
    {
        Guard.NotNull(source, paramName);
        if (source.Count() > 0)
        {
            return source.Sum(selector);
        }
        return default;
    }

    public static decimal SumOrDefault<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, decimal> selector,
        [CallerArgumentExpression(nameof(source))] string? paramName = null)
    {
        Guard.NotNull(source, paramName);
        if (source.Count() > 0)
        {
            return source.Sum(selector);
        }
        return default;
    }

    public static double? SumOrDefault(this IEnumerable<double?> source,
        [CallerArgumentExpression(nameof(source))] string? paramName = null)
    {
        Guard.NotNull(source, paramName);

        if (source.Count() > 0)
        {
            return source.Sum();
        }
        return default;
    }

    public static float? SumOrDefault(this IEnumerable<float?> source,
        [CallerArgumentExpression(nameof(source))] string? paramName = null)
    {
        Guard.NotNull(source, paramName);
        if (source.Count() > 0)
        {
            return source.Sum();
        }
        return default;
    }

    public static decimal SumOrDefault(this IEnumerable<decimal> source,
        [CallerArgumentExpression(nameof(source))] string? paramName = null)
    {
        Guard.NotNull(source, paramName);
        if (source.Count() > 0)
        {
            return source.Sum();
        }
        return default;
    }

    public static double SumOrDefault(this IEnumerable<double> source,
        [CallerArgumentExpression(nameof(source))] string? paramName = null)
    {
        Guard.NotNull(source, paramName);
        if (source.Count() > 0)
        {
            return source.Sum();
        }
        return default;
    }

    public static int SumOrDefault(this IEnumerable<int> source,
        [CallerArgumentExpression(nameof(source))] string? paramName = null)
    {
        Guard.NotNull(source, paramName);
        if (source.Count() > 0)
        {
            return source.Sum();
        }
        return default;
    }
    //     source is null.
    public static float SumOrDefault(this IEnumerable<float> source,
        [CallerArgumentExpression(nameof(source))] string? paramName = null)
    {
        Guard.NotNull(source, paramName);
        if (source.Count() > 0)
        {
            return source.Sum();
        }
        return default;
    }

    public static decimal? SumOrDefault(this IEnumerable<decimal?> source,
        [CallerArgumentExpression(nameof(source))] string? paramName = null)
    {
        Guard.NotNull(source, paramName);
        if (source.Count() > 0)
        {
            return source.Sum();
        }
        return default;
    }

    public static int? SumOrDefault(this IEnumerable<int?> source,
        [CallerArgumentExpression(nameof(source))] string? paramName = null)
    {
        Guard.NotNull(source, paramName);
        if (source.Count() > 0)
        {
            return source.Sum();
        }
        return default;
    }

    public static long? SumOrDefault(this IEnumerable<long?> source,
        [CallerArgumentExpression(nameof(source))] string? paramName = null)
    {
        Guard.NotNull(source, paramName);
        if (source.Count() > 0)
        {
            return source.Sum();
        }
        return default;
    }

    public static long SumOrDefault(this IEnumerable<long> source,
        [CallerArgumentExpression(nameof(source))] string? paramName = null)
    {
        Guard.NotNull(source, paramName);
        if (source.Count() > 0)
        {
            return source.Sum();
        }
        return default;
    }
    #endregion

    #region String

    public static bool Contains(this List<string> source,
        string value, StringComparison stringComparison,
        [CallerArgumentExpression(nameof(source))] string? paramName = null)
    {
        Guard.NotNull(source, paramName);
        return source.IndexOf(value, stringComparison) >= 0;
    }

    public static bool Contains(this string[] source, string value, StringComparison stringComparison,
        [CallerArgumentExpression(nameof(source))] string? paramName = null)
    {
        Guard.NotNull(source, paramName);
        return source.IndexOf(value, stringComparison) >= 0;
    }

    public static int IndexOf(this List<string> source, string value, StringComparison stringComparison,
        [CallerArgumentExpression(nameof(source))] string? paramName = null)
    {
        Guard.NotNull(source, paramName);
        for (var i = 0; i < source.Count; i++)
        {
            var item = source[i];
            if (item != null)
            {
                if (item.Equals(value, stringComparison))
                {
                    return i;
                }
            }
            else
            {
                if (item == value)
                {
                    return i;
                }
            }
        }
        return -1;
    }

    public static int IndexOf(this string[] source, string value, StringComparison stringComparison,
        [CallerArgumentExpression(nameof(source))] string? paramName = null)
    {
        Guard.NotNull(source, paramName);
        for (var i = 0; i < source.Length; i++)
        {
            var item = source[i];
            if (item != null)
            {
                if (item.Equals(value, stringComparison))
                {
                    return i;
                }
            }
            else
            {
                if (item == value)
                {
                    return i;
                }
            }
        }
        return -1;
    }
    #endregion

    public static HashSet<T?> ToHashSet<T>(
        this IEnumerable<T?> colecao,
        bool aceitarNull,
        [CallerArgumentExpression(nameof(colecao))] string? paramName = null) where T : class
    {
        Guard.NotNull(colecao, paramName);
        var hashSet = colecao.ToHashSet();
        if (aceitarNull)
        {
            return hashSet;
        }
        hashSet.Remove(null);
        return hashSet;
    }

    private static bool IsConcurrentCollection(IEnumerable enumerable)
    {
        var type = enumerable.GetType();
        if (type.IsGenericType)
        {
            var typeDefinition = type.GetGenericTypeDefinition();
            return typeDefinition == typeof(ConcurrentDictionary<,>) ||
                   typeDefinition == typeof(ConcurrentQueue<>) ||
                   typeDefinition == typeof(ConcurrentStack<>) ||
                   typeDefinition == typeof(ConcurrentBag<>);
        }
        return false;
    }

    public static T? PeekOrDefault<T>(this Queue<T> queue,
        [CallerArgumentExpression(nameof(queue))] string? paramName = null)
    {
        Guard.NotNull(queue, paramName);
        if (queue.Count > 0)
        {
            return queue.Peek();
        }
        return default;
    }

    public static string StringJoin(this IEnumerable<string?> source, string separator)
    {
        if (source == null || !source.Any())
        {
            return string.Empty;
        }
        return string.Join(separator, source);
    }

    public static string StringJoin<T>(
        this IEnumerable<T> source,
        Func<T, string?> predictate, string separator)
    {
        if (source == null || !source.Any())
        {
            return string.Empty;
        }
        return string.Join(separator, source.Select(predictate));
    }

#if NET45 || NET40
    public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, T item)
    {
        yield return item;
        foreach (var element in source)
        {
            yield return element;
        }
    }

    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> colecao)
    {
        return new HashSet<T>(colecao);
    }
#endif

    public static int CountEnumerable(this IEnumerable enumerable)
    {
        return enumerable.Cast<object>().Count();
    }

    public static List<T> ToList<T>(this IEnumerable enumerable)
    {
        return enumerable
            .Cast<T>()
            .ToList();
    }
}