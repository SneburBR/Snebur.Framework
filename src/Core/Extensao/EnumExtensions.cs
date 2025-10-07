using Snebur.Helpers;
using System.ComponentModel;
using System.Reflection;

namespace Snebur;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        if (field == null)
            return value.ToString();

        var labelAttribute = field.GetCustomAttribute<RotuloAttribute>();
        if (labelAttribute is not null)
            return labelAttribute.Rotulo;
        var attribute = field.GetCustomAttribute<DescriptionAttribute>();

        return attribute != null
            ? attribute.Description
            : value.ToString();
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
