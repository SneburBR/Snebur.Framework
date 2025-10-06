using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Snebur;

public class IgnorarCasoSensivel : IEqualityComparer<string>
{
    public bool Equals(string? x, string? y)
    {
        if (x is null && y is null)
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }
        return x.ToLowerInvariant() == y.ToLowerInvariant();
    }

    public int GetHashCode(string obj)
    {
        if (obj == null)
        {
            return 0;
        }
        return obj.ToLower().GetHashCode();
    }
}

public class IgnorarCasoSensivelCaracter : IEqualityComparer<char>
{
    public bool Equals(char x, char y)
    {
        return Char.ToLowerInvariant(x) == Char.ToLowerInvariant(y);
    }

    public int GetHashCode(char obj)
    {
        return Char.ToLowerInvariant(obj).GetHashCode();
    }
}

public class CaseAndAccentInsensitiveStringComparer : IEqualityComparer<string?>
{
    private readonly CompareInfo _compareInfo;

    public CaseAndAccentInsensitiveStringComparer(CultureInfo? culture)
    {
        _compareInfo = (culture ?? CultureInfo.InvariantCulture).CompareInfo;
    }

    public bool Equals(string? s1, string? s2)
        => _compareInfo.Compare(s1, s2, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0;

    public int GetHashCode([DisallowNull] string? obj)
        => _compareInfo.GetHashCode(obj, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase);
}

public static class StringCompareres
{
    public static CaseAndAccentInsensitiveStringComparer CaseAndAccentInsensitive
        => field ??= new(new CultureInfo("pt-BR"));
}