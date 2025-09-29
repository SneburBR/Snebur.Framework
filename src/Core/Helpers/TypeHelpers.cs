using System.Diagnostics.CodeAnalysis;

namespace Snebur.Helpers;

public static class TypeHelpers
{
    public static T GetUnderlyingDefaultValue<T>()
    {
        var type = typeof(T);
        var underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType is not null)
        {
            return (T)Activator.CreateInstance(underlyingType)!;
        }
        return default!;
    }

    public static object? GetDefaultValue(Type tipo)
    {
        if (!tipo.IsValueType)
        {
            return null;
        }

        if (tipo.IsGenericType &&
            tipo.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return null;
        }
        return Activator.CreateInstance(tipo);
    }

    public static bool TryChangeType(
        object? parsed,
        Type underlying,
        [NotNullWhen(true)]
        out object? enumValue)
    {
        try
        {
            if (parsed is null)
            {
                enumValue = GetUnderlyingDefaultValue<object>();
                return true;
            }
            enumValue = Convert.ChangeType(parsed, underlying);
            return true;
        }
        catch
        {
            enumValue = null;
            return false;
        }
    }
}
