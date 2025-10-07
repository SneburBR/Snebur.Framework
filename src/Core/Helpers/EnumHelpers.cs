using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
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
    public static bool IsFlagsType(Type type)
        => CustomAttributeExtensions.GetCustomAttribute<FlagsAttribute>(type) is not null;

    public static bool IsFlags<T>()
        where T : struct, Enum
        => CustomAttributeExtensions.GetCustomAttribute<FlagsAttribute>(typeof(T)) is not null;

    public static TEnum GetEnumValue<TEnum>(object? valueOrName)
        where TEnum : struct, Enum
    {
        return (TEnum)GetEnumValue(typeof(TEnum), valueOrName);
    }

    public static Enum GetEnumValue(Type enumType, object? valueOrName)
    {
        if (valueOrName is null)
        {
            return EnumHelpers.GetRequiredEnumUndefinedValue(enumType);
        }

        if (valueOrName is Enum enumValue)
        {
            if (Enum.IsDefined(enumType, valueOrName))
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

        var valueType = valueOrName.GetType();
        if (valueType.IsNumberType())
        {
            if (IsDefined(enumType, valueOrName))
            {
                var enumValueType = Enum.GetUnderlyingType(enumType);
                if (enumValueType != valueType)
                {
                    valueOrName = Convert.ChangeType(valueOrName, enumValueType);
                }
                if (Enum.IsDefined(enumType, valueOrName))
                {
                    return (Enum)Enum.ToObject(enumType, valueOrName);
                }
            }
        }

        Debugger.Break();
        throw new Erro($"O valor {valueOrName} do tipo {valueType.GetType().Name} não é suportado para o enum {enumType.Name}");
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

        foreach (var fallbackValue in new int[] { -1 })
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
        var enumEnderlineType = Enum.GetUnderlyingType(enumType);
        if (enumEnderlineType != value.GetType())
        {
            value = Convert.ChangeType(value, enumEnderlineType);
        }

        if (!Enum.IsDefined(enumType, value))
            return false;

        Enum enumValue = (Enum)Enum.ToObject(enumType, value);
        var undef = GetEnumUndefinedValue(enumType);
        if (undef is not null && Equals(undef, enumValue))
        {
            return false;
        }
        return true;
    }

    public static bool IsDefined<TEnum>(TEnum value)
         where TEnum : struct, Enum
    {
        var enumType = typeof(TEnum);
        var undef = GetEnumUndefinedValue(enumType);
        if (undef is not null && Equals(undef, value))
            return false;

        return Enum.IsDefined(value);
    }

    public static T GetAllFlags<T>() where T : Enum
    {
        return (T)GetAllFlags(typeof(T));
    }

    public static Enum GetAllFlags(Type enumType)
    {
        if (IsFlagsType(enumType) == false)
            throw new ArgumentException($"Type {enumType} is not a flags enum. Apply {nameof(FlagsAttribute)} to the enum definition.");

        ulong all = 0;
        foreach (var val in Enum.GetValues(enumType))
        {
            all |= Convert.ToUInt64(val);
        }
        return (Enum)Enum.ToObject(enumType, all);
    }

    public static bool IsDefined(Enum enumValue)
    {
        return IsDefined(enumValue.GetType(), enumValue);
    }

    private static void EnsureIsEnum(Type type)
    {
        if (!type.IsEnum)
            throw new ArgumentException($"Type {type} is not an enum.");
    }
}
