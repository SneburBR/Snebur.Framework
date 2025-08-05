using System.Globalization;

namespace Snebur.Utilidade;

public static class CsSharpLiteralValueUtils
{
    public static string GetLiteralValue(this object? defaultValue)
    {
        if (defaultValue is null)
        {
            return "null";
        }

        if (defaultValue.GetType().IsEnum == true)
        {
            return GetEnumLiteralValue((Enum)defaultValue);
        }

        return defaultValue switch
        {
            string str => $"\"{str}\"",
            bool b => b ? "true" : "false",
            int i => i.ToString(CultureInfo.InvariantCulture),
            long l => l.ToString(CultureInfo.InvariantCulture) + "L",
            decimal d => d.ToString(CultureInfo.InvariantCulture) + "m",
            double dbl => dbl.ToString(CultureInfo.InvariantCulture) + "d",
            float f => f.ToString(CultureInfo.InvariantCulture) + "f",
            byte bt => bt.ToString(CultureInfo.InvariantCulture),
            char ch => $"'{EscapeChar(ch)}'",
            Guid guid => $"new Guid(\"{guid}\")",
            DateTime dt =>
               $"new DateTime({dt.Year}, {dt.Month}, {dt.Day}, {dt.Hour}, {dt.Minute}, {dt.Second}, DateTimeKind.{dt.Kind})",
            TimeSpan ts => $"new TimeSpan({ts.Ticks})",
            _ => throw new NotSupportedException($"Tipo de valor padrão '{defaultValue.GetType().Name}' não suportado.")
        };
    }

   

    private static string GetEnumLiteralValue(Enum defaultValue)
    {
        var enumType = defaultValue.GetType();
        var enumName = Enum.GetName(enumType, defaultValue);
        if (enumName == null)
        {
            throw new Exception($"Enum '{enumType.Name}' não contém o valor '{defaultValue}'.");
        }
        return $"{enumType.Name}.{enumName}";

    }
    private static string EscapeChar(char c)
    {
        return c switch
        {
            '\'' => @"\'",
            '\"' => "\\\"",
            '\\' => @"\\",
            '\0' => @"\0",
            '\a' => @"\a",
            '\b' => @"\b",
            '\f' => @"\f",
            '\n' => @"\n",
            '\r' => @"\r",
            '\t' => @"\t",
            '\v' => @"\v",
            _ => char.IsControl(c) ? $"\\u{(int)c:x4}" : c.ToString()
        };
    }
}
