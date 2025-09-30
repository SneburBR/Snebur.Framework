using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

namespace Snebur.Helpers;

public static class EnumHelpers
{
    private static readonly ConcurrentDictionary<Type, Enum?> _cache = new();

    public const string UNDEFINED_NAME = "Undefined";
    public const int UNDEFINED_VALUE = -1;

    private static readonly string[] FallbackNames = {
        "Undefined",
        "None",
        "Unknown",
        "NotDefined",
        "NotDefinedValue",
        "Indefinido",
        "NaoDefinido",
        "NaoDefinida",
        "Desconhecido",
        "Desconhecida",
        "Nenhum",
        "Nenhuma",
        "Nada",
    };
    public static bool IsFlags(Type type)
        => type.GetCustomAttribute<FlagsAttribute>() is not null;

    public static bool IsFlags<T>()
        where T : struct, Enum
        => typeof(T).GetCustomAttribute<FlagsAttribute>() is not null;

    public static Enum GetEnumValue(Type enumType, object? valueOrName)
    {
        if (valueOrName is null)
        {
            return EnumHelpers.GetRequiredEnumUndefinedValue(enumType);
        }

        if (valueOrName is Enum enumValue)
        {
            if (Enum.IsDefined(enumType, enumValue) || Attribute.IsDefined(enumType, typeof(FlagsAttribute)))
            {
                return enumValue;
            }
        }

        if (valueOrName is string enumName)
        {
            if (Enum.TryParse(enumType, enumName, ignoreCase: true, out object? parsed))
            {
                return (Enum)parsed!;
            }
        }

        if (valueOrName is int intValue)
        {
            if (Enum.IsDefined(enumType, intValue))
            {
                return (Enum)Enum.ToObject(enumType, intValue);
            }

            if (Attribute.IsDefined(enumType, typeof(FlagsAttribute)))
            {
                return (Enum)Enum.ToObject(enumType, intValue);
            }
        }

        Debugger.Break();
        throw new Erro($"O valor {valueOrName} não é suportado para o enum {enumType.Name}");
    }

    public static Enum GetRequiredEnumUndefinedValue(Type enumType)
    {
        return GetEnumUndefinedValue(enumType)
            ?? throw new Exception($"Is not possible to get undefined value for enum {enumType.Name}, define a value with {nameof(UndefinedEnumValueAttribute)} attribute or a value with 0 or -1");
    }

    public static Enum? GetEnumUndefinedValue(Type enumType)
    {
        Guard.NotNull(enumType);
        EnsureIsEnum(enumType);
        return _cache.GetOrAdd(enumType, ResolveUndefinedValue);
    }

    public static T GetEnumUndefinedValue<T>()
        where T : struct, Enum
    {
        var type = typeof(T);
        var result = _cache.GetOrAdd(type, ResolveUndefinedValue) as T?;
        return result
            ?? throw new Exception(
                $"Is not possible to get undefined value for enum {type.Name}, define a value with {nameof(UndefinedEnumValueAttribute)} attribute or a value with 0 or -1");
    }

    private static Enum? ResolveUndefinedValue(Type enumType)
    {

        var fields = enumType.GetFields();
        var fieldUndefined = fields.FirstOrDefault(f => f.GetCustomAttribute<UndefinedEnumValueAttribute>(true) != null);
        if (fieldUndefined != null)
        {
            return (Enum)fieldUndefined.GetValue(null)!;
        }

        foreach (var fallbackValue in new int[] { -1, 0 })
        {
            try
            {
                if (Enum.IsDefined(enumType, fallbackValue))
                {
                    return (Enum)Enum.ToObject(enumType, fallbackValue);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error trying to get undefined value for enum {enumType.Name} with fallback value {fallbackValue}", ex);
            }
        }

        foreach (var fallbackName in FallbackNames)
        {
            if (Enum.TryParse(enumType, fallbackName, ignoreCase: true, out object? parsed))
            {
                return (Enum)parsed;
            }
        }
        return null;
    }
    public static bool IsDefined(Type enumType, object value)
    {
        if (IsFlags(enumType))
            return true;

        if (Enum.IsDefined(enumType, value))
            return true;

        if (IsFlags(enumType))
            return true;

        var undef = GetEnumUndefinedValue(enumType);
        return undef is not null && !Equals(undef, value);
    }

    public static bool IsDefined<TEnum>(TEnum value)
         where TEnum : struct, Enum
    {
        if (Enum.IsDefined(value))
            return true;

        var enumType = typeof(TEnum);
        if (IsFlags(enumType))
            return true;

        var undef = GetEnumUndefinedValue(enumType);
        return undef is not null && !Equals(undef, value);
    }
    private static void EnsureIsEnum(Type type)
    {
        if (!type.IsEnum)
            throw new ArgumentException($"Type {type} is not an enum.");
    }
}