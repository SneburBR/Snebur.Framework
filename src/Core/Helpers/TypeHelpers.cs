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

    public static object? CreateDefaultValue(Type type)
    {
        Guard.NotNull(type);

        return type.IsValueType
            ? Activator.CreateInstance(type)
            : null;
    }

    public static object CreateNotNullDefaultValue(Type type)
    {
        Guard.NotNull(type);

        if (type.IsEnum)
        {
            return EnumHelpers.GetRequiredEnumUndefinedValue(type);
        }

        if (type.IsValueType)
        {
            return Activator.CreateInstance(type)!;
        }

        if (type == typeof(string))
        {
            return string.Empty;
        }

        if (type.IsArray)
        {
            return Array.CreateInstance(type.GetElementType()!, 0);
        }

        if (type.IsGenericType)
        {
            var genericDefinition = type.GetGenericTypeDefinition();
            if (genericDefinition == typeof(List<>) ||
                genericDefinition == typeof(Dictionary<,>) ||
                genericDefinition == typeof(HashSet<>))
            {
                return Activator.CreateInstance(type)!;
            }
        }

        if (!type.IsConcrete())
        {
            throw new InvalidCastException(
                $"Cannot create a default non-null value for type '{type.FullName}' because it's not concrete.");
        }

        try
        {
            return Activator.CreateInstance(type)!;
        }
        catch (Exception ex)
        {
            throw new InvalidCastException(
              $"Cannot create a default non-null value for type '{type.FullName}'.", ex);
        }
    }
}
