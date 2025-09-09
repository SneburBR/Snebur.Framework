namespace Snebur.Utilidade;

public static class LazyUtil
{
    private static readonly object _bloqueioLazy = new object();

    public static T RetornarValorLazyComBloqueio<T>(ref T? valor, Func<T> retornarValor) where T : struct
    {
        if (!valor.HasValue)
        {
            lock (_bloqueioLazy)
            {
                if (!valor.HasValue)
                {
                    valor = retornarValor.Invoke();
                }
            }
        }
        return valor.Value;
    }

    public static T RetornarValorLazyComBloqueio<T>(ref T? valor, Func<T> retornarValor) where T : class
    {
        if (valor == null)
        {
            lock (_bloqueioLazy)
            {
                if (valor == null)
                {
                    valor = retornarValor.Invoke();
                }
            }
        }
        return valor;
    }

    public static T RetornarValorLazy<T>(ref T? valor, Func<T> retornarValor) where T : struct
    {
        if (!valor.HasValue)
        {
            lock (_bloqueioLazy)
            {
                if (!valor.HasValue)
                {
                    valor = retornarValor.Invoke();
                }
            }
        }
        return valor.Value;
    }

    public static T RetornarValorLazy<T>(
        ref T? valor,
        Func<T> retornarValor) where T : class
    {
        if (valor is null)
        {
            var novoValor = retornarValor.Invoke();
            if (valor is null)
            {
                Guard.NotNull(novoValor);
                valor = novoValor;
            }
        }
        return valor;
    }

    public static T? RetornarValorLazyTalvezNulo<T>(
        ref T? valor,
        Func<T?> retornarValor) where T : class
    {
        if (valor is null)
        {
            var novoValor = retornarValor.Invoke();
            if (valor is null)
            {
                valor = novoValor;
            }
        }
        return valor;
    }
}