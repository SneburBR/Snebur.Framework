using Snebur.Helpers;
using System.ComponentModel;
using System.Reflection;

namespace Snebur;

public static class EnumExtensions
{
    public static bool IsHasUnderfinedAttribute(this Enum value)
    {
        return value.GetAttribute<UndefinedEnumValueAttribute>() != null;
    }

    public static string GetDescription(this Enum value)
    {
        return value.GetAttribute<RotuloAttribute>()?.Rotulo
            ?? value.ToString()
            ?? value.GetAttribute<DescriptionAttribute>()?.Description
            ?? value.ToString();
    }
    public static TAtribute? GetAttribute<TAtribute>(this Enum value)
        where  TAtribute : Attribute

    {
        var field = value.GetType().GetField(value.ToString());
        if (field == null)
            return null;

        return field.GetCustomAttribute<TAtribute>();
   
    }

    public static TEnum FallbackIfNotDefined<TEnum>(this TEnum value, TEnum fallback) where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(typeof(TEnum), value))
            return fallback;

        return value;
    }
    public static TEnum? FallbackIfNotDefined<TEnum>(this TEnum? value, TEnum? fallback) where TEnum : struct, Enum
    {
        if (value == null)
            return fallback;

        if (!Enum.IsDefined(typeof(TEnum), value))
            return fallback;

        return value;
    }

    public static bool IsDefined(this Enum value)
    {
        return EnumHelpers.IsDefined(value.GetType(), value);
    }

    public static string GetName(this Enum value)
    {
        return Enum.GetName(value.GetType(), value) ?? value.ToString();
    }
}
