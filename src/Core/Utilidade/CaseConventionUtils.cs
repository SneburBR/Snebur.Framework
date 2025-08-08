namespace Snebur.Utilidade;

public static partial class CaseConventionUtils
{
    [GeneratedRegex("^[A-Z0-9]+$")]
    private static partial Regex UpperCaseRegex();

    [GeneratedRegex("^[a-z0-9]+$")]
    private static partial Regex LowerCaseRegex();

    [GeneratedRegex("^[A-Z]+[a-z0-9]+(?:[A-Z][a-z0-9]*)*$")]
    private static partial Regex PascalCaseRegex();

    [GeneratedRegex("^[a-z][a-z0-9]*(?:[A-Z][a-z0-9]*)*$")]
    private static partial Regex CamelCaseRegex();

    [GeneratedRegex("^[a-z0-9]+(?:_[a-z0-9]+)*$")]
    private static partial Regex SnakeCaseRegex();

    [GeneratedRegex("^[a-z0-9]+(?:-[a-z0-9]+)*$")]
    private static partial Regex KebabCaseRegex();

    [GeneratedRegex("^[A-Z0-9_]+$")]
    private static partial Regex ScreamingSnakeCaseRegex();

    [GeneratedRegex("^[A-Z0-9-]+$")]
    public static partial Regex ScreamingKebabCaseRegex();

    [GeneratedRegex("^[A-Z][a-z0-9]*(?:_[A-Z][a-z0-9]*)*$")]
    public static partial Regex TitleSnakeCaseRegex();

    [GeneratedRegex("^[A-Z][a-z0-9]*(?:-[A-Z][a-z0-9]*)*$")]
    public static partial Regex TitleTitleKebabCaseRegex();

    [GeneratedRegex("^[a-z][a-z0-9]*(?:_[A-Z][a-z0-9]*)*$")]
    public static partial Regex CamelSnakeCaseRegex();

    [GeneratedRegex("^[a-z][a-z0-9]*(?:-[A-Z][a-z0-9]*)*$")]
    public static partial Regex CamelKebabCaseRegex();

    [GeneratedRegex(@"[^A-Za-z0-9]+")]
    private static partial Regex SpliPartsNoLetterOrDigitRegex();

    [GeneratedRegex(@"([A-Z]+(?=[A-Z][a-z]))|([A-Z]?[a-z]+)|([A-Z]+)|(\d+)")]
    private static partial Regex SplitRegex();

    public static CaseType GetCaseType(string? input)
    {
        Guard.NotNullOrWhiteSpace(input);

        if (string.IsNullOrWhiteSpace(input))
            return CaseType.Unknown;

        if (LowerCaseRegex().IsMatch(input))
            return CaseType.LowerCase;

        if (UpperCaseRegex().IsMatch(input))
            return CaseType.UpperCase;

        if (PascalCaseRegex().IsMatch(input))
            return CaseType.PascalCase;

        if (CamelCaseRegex().IsMatch(input))
            return CaseType.CamelCase;

        if (input.Contains('_', StringComparison.Ordinal) &&
            SnakeCaseRegex().IsMatch(input))
            return CaseType.SnakeCase;

        if (KebabCaseRegex().IsMatch(input))
            return CaseType.KebabCase;

        if (ScreamingKebabCaseRegex().IsMatch(input))
            return CaseType.ScreamingKebabCase;

        if (ScreamingSnakeCaseRegex().IsMatch(input))
            return CaseType.ScreamingSnakeCase;

        if (TitleSnakeCaseRegex().IsMatch(input))
            return CaseType.TitleSnakeCase;

        if (TitleTitleKebabCaseRegex().IsMatch(input))
            return CaseType.TitleKebabCase;

        if (CamelSnakeCaseRegex().IsMatch(input))
            return CaseType.CamelSnakeCase;

        if (CamelKebabCaseRegex().IsMatch(input))
            return CaseType.CamelKebabCase;

        return CaseType.Unknown;
    }

    public static string ToSnakeCase(string? input)
    {
        Guard.NotNullOrWhiteSpace(input);

        var words = SplitWords(input);
        return string.Join("_", words).ToLowerInvariant();
    }

    public static string ToKebabCase(string? input)
    {
        Guard.NotNullOrWhiteSpace(input);
        var words = SplitWords(input);
        return string.Join("-", words).ToLowerInvariant();
    }

    public static string ToPascalCase(string? input)
    {
        Guard.NotNullOrWhiteSpace(input);

        var words = SplitWords(input);
        return string.Join("", words.Select(x => x.Capitalize()));
    }

    public static string ToCamelCase(string? input)
    {
        Guard.NotNullOrWhiteSpace(input);
        var words = SplitWords(input);
        return string.Join("", words.Select((x, i) => i == 0 ? x.Uncapitalize() : x.Capitalize()));
    }

    public static string ToTitleSnakeCase(string? input)
    {
        Guard.NotNullOrWhiteSpace(input);
        var words = SplitWords(input);
        return string.Join("_", words.Select(x => x.Capitalize()));
    }

    public static string ToScreamingSnakeCase(string? input)
    {
        Guard.NotNullOrWhiteSpace(input);
        var words = SplitWords(input);
        return string.Join("_", words.Select(x => x.ToUpperInvariant()));
    }

    public static string ToUpperCase(string? input)
    {
        Guard.NotNullOrWhiteSpace(input);
        var words = SplitWords(input);

        return string.Join(string.Empty, words).ToUpperInvariant();
    }

    public static string[] SplitWords(string input)
    {
        //Split the input into parts that contain no letters or digits.
        string[] parts = SpliPartsNoLetterOrDigitRegex().Split(input);
        var result = new List<string>();

        // Process each part separated
        foreach (var token in parts)
        {
            // This regex extracts words from camelCase, PascalCase, or ALLCAPS tokens.
            // It matches:
            // - groups of uppercase letters that are followed by another uppercase letter and then a lowercase letter
            // - an optional uppercase letter followed by one or more lowercase letters
            // - or one or more uppercase letters (for acronyms/numbers)

            var matches = SplitRegex().Matches(token);
            foreach (Match m in matches)
            {
                result.Add(m.Value);
            }
        }
        return result.ToArray();
    }
}
public enum CaseType
{
    Unknown,
    LowerCase,
    PascalCase,
    CamelCase,
    SnakeCase,
    KebabCase,
    ScreamingSnakeCase,
    ScreamingKebabCase,
    TitleSnakeCase,
    TitleKebabCase,
    CamelSnakeCase,
    CamelKebabCase,
    UpperCase,
}