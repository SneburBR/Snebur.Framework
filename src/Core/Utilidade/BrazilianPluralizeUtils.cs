namespace Snebur.Utilidade;
//disable spelling and grammar check

public static class BrazilianPluralizeUtils
{
    private static readonly Dictionary<string, string> IrregularPluralForms = new(StringComparer.OrdinalIgnoreCase)
    {
         // “ão” words with accents and without
         { "alemão",   "alemães"   },
         { "alemao",   "alemaes"   },

         { "ancião",   "anciãos"   },
         { "anciao",   "anciaos"   },

         { "capitão",  "capitães"  },
         { "capitao",  "capitaes"  },

         { "charlatão","charlatães"},
         { "charlatao","charlataes"},

         { "cidadão",  "cidadãos"  },
         { "cidadao",  "cidadaos"  },

         { "cristão",  "cristãos"  },
         { "cristao",  "cristaos"  },

         { "cão",      "cães"      },
         { "cao",      "caes"      },

         { "grão",     "grãos"     },
         { "grao",     "graos"     },

         { "irmão",    "irmãos"    },
         { "irmao",    "irmaos"    },

         { "mão",      "mãos"      },
         { "mao",      "maos"      },

         { "órfão",    "órfãos"    },
         { "orfao",    "orfaos"    },

         { "órgão",    "órgãos"    },
         { "orgao",    "orgaos"    },

         { "pão",      "pães"      },
         { "pao",      "paes"      },

         { "refrão",   "refrães"   },
         { "refrao",   "refraes"   },

         // other common irregulars (accented and unaccented)
         { "cônsul",   "cônsules"  },
         { "consul",   "consules"  },

         { "fóssil",   "fósseis"   },
         { "fossil",   "fosseis"   },

         { "mal",      "males"     },  // invariant form “mal” → “males”
         // no accent to unaccent needed for “mal”

         { "réptil",   "répteis"   },
         { "reptil",   "repteis"   }
    };

    public static string? Pluralize(string? word)
    {
        if (string.IsNullOrWhiteSpace(word))
        {
            return word;
        }
        if (string.IsNullOrWhiteSpace(word))
        {
            return word;
        }

        var puralForm = PluralizeInternal(word);
        return PreserveCapitalization(word, puralForm);
    }

    private static string PluralizeInternal(string word)
    {
        var lowerCaseWord = word.Trim().ToLowerInvariant();

        // Check irregular dictionary
        if (IrregularPluralForms.TryGetValue(lowerCaseWord, out var irregularPlural))
        {
            return irregularPlural;
        }

        // Words ending in "ão" → replace with "ões"
        if (lowerCaseWord.EndsWith("ão", StringComparison.Ordinal))
        {
            return word.Substring(0, word.Length - 3) + "ões";
        }
        // Words ending in "l" → replace "l" with "is"
        if (lowerCaseWord.EndsWith("l", StringComparison.Ordinal))
        {
            return word.Substring(0, word.Length - 1) + "is";
        }

        // Words ending in "m" → replace with "ns"
        if (lowerCaseWord.EndsWith("m", StringComparison.Ordinal))
        {
            return word.Substring(0, word.Length - 1) + "ns";
        }
        // Words ending in "s" → usually invariant
        if (lowerCaseWord.EndsWith("s", StringComparison.Ordinal))
        {
            return word;
        }

        // Words ending in "r", "z", "x" → add "es"
        if ("rzx".Contains(lowerCaseWord[^1]))
        {
            return word + "es";
        }

        // Words ending in a vowel (a, e, i, o, u) → add "s"
        if ("aeiou".Contains(lowerCaseWord[^1]))
        {
            return word + "s";
        }
        // Fallback: add "s"
        return word + "s";
    }

    private static string PreserveCapitalization(string original, string pluralForm)
    {
        if (pluralForm.IsCapitalized())
        {
            return pluralForm.Capitalize();
        }
        return pluralForm;
    }
}
