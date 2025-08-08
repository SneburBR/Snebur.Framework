namespace Snebur;

public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string? value)
    {
        return string.IsNullOrEmpty(value);
    }

    public static bool IsNullOrWhiteSpace(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    public static string GetOnlyNumbers(this string? value)
    {
        if (value is null)
            return string.Empty;

        return new string(value.Where(char.IsDigit).ToArray());
    }

    public static string GetOnlyNumbers(this string? value, string? allowedExtraChars)
    {
        if (value is null)
            return string.Empty;

        if (allowedExtraChars is null)
            return value.GetOnlyNumbers();

        return value.GetOnlyNumbers(allowedExtraChars.ToCharArray());
    }

    public static string GetOnlyNumbers(this string? value, params char[] allowedExtraChars)
    {
        if (value is null)
            return string.Empty;

        Guard.NotNull(allowedExtraChars);

        if (allowedExtraChars.Length > 10)
        {
            var allowedChars = new HashSet<char>(allowedExtraChars);
            return new string(value.Where(ch => char.IsDigit(ch) || allowedChars.Contains(ch)).ToArray());
        }

        return new string(value.Where(ch => char.IsDigit(ch) || allowedExtraChars.Contains(ch)).ToArray());
    }

    public static string GetOnlyLetters(this string? value)
    {
        if (value is null)
            return string.Empty;

        return new string(value.Where(char.IsLetter).ToArray());
    }

    public static string GetOnlySpecialChars(this string? value)
    {
        if (value is null)
            return string.Empty;

        return new string(value.Where(ch => !char.IsLetterOrDigit(ch)).ToArray());
    }

    public static string GetOnlyLettersAndNumbers(this string? value)
    {
        if (value is null)
            return string.Empty;

        return new string(value.Where(char.IsLetterOrDigit).ToArray());
    }

    public static string SafeSubstring(this string? value, int startIndex)
    {
        if (value is null)
            return string.Empty;

        if (startIndex < 0)
            startIndex = 0;

        if (startIndex > value.Length)
            return string.Empty;

        return value.Substring(startIndex);
    }

    public static string SafeSubstring(this string? value, int startIndex, int length)
    {
        if (value is null)
            return string.Empty;

        if (startIndex < 0)
            startIndex = 0;

        if (length < 0)
            length = 0;

        if (startIndex + length > value.Length)
            length = value.Length - startIndex;

        return value.Substring(startIndex, length);
    }

    public static string SafeTrim(this string? value,
        int maxLength,
        string? appendWhenTrimmed = null)
    {
        if (value is null)
            return string.Empty;

        var trimmedValue = value.Trim();
        return maxLength > 0 && value.Length > maxLength
            ? $"{trimmedValue.Substring(0, maxLength)}{appendWhenTrimmed}"
            : value;
    }

    public static string Capitalize(this string? value)
    {
        if (value is null)
            return string.Empty;

        value = value.TrimStart();
        if (value.Length == 0)
            return string.Empty;

        return char.ToUpperInvariant(value[0]) + value.Substring(1);
    }

    public static string Uncapitalize(this string? value)
    {
        if (value is null)
            return string.Empty;

        value = value.TrimStart();
        if (value.Length == 0)
            return string.Empty;

        if(value == value.ToUpperInvariant())
        {
            return value.ToLowerInvariant();
        }
        return char.ToLowerInvariant(value[0]) + value.Substring(1);
    }

    public static bool IsCapitalized(this string? value)
    {
        if (value is null || value.Length == 0)
            return false;

        return char.IsUpper(value[0]);
    }
}
