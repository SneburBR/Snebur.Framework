
namespace Snebur.Utilidade;

public static class BrazilianPluralizeUtils
{
    public static string? Pluralize(string? word)
    {
        if(string.IsNullOrWhiteSpace(word))
        {
            return word;
        }
        
        var lower = word.ToLowerInvariant();

        // 1. Words ending in "ão" → replace with "ões"
        if (lower.EndsWith("ão", StringComparison.Ordinal))
            return word.Substring(0, word.Length - 2) + "ões";

        // 2. Words ending in "l" → replace "l" with "is"
        if (lower.EndsWith("l", StringComparison.Ordinal))
            return word.Substring(0, word.Length - 1) + "is";

        // 3. Words ending in "m" → replace "m" with "ns"
        if (lower.EndsWith("m", StringComparison.Ordinal))
            return word.Substring(0, word.Length - 1) + "ns";

        // 4. Words ending in "r", "z" or "x" → add "es"
        if (lower.EndsWith("r", StringComparison.Ordinal)
         || lower.EndsWith("z", StringComparison.Ordinal)
         || lower.EndsWith("x", StringComparison.Ordinal))
            return word + "es";

        // 5. Words ending in a vowel (a, e, i, o, u) → add "s"
        char lastChar = lower[lower.Length - 1];
        if ("aeiou".IndexOf(lastChar) >= 0)
            return word + "s";

        // 6. Words ending in "s" (invariant in most cases) → leave as is
        if (lower.EndsWith("s", StringComparison.Ordinal))
            return word;

        // 7. Fallback → add "s"
        return word + "s";
    }
}
