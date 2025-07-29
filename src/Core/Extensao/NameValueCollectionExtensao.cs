using System.Collections.Specialized;

namespace Snebur;

public static class NameValueCollectionExtensao
{
    public static string? GetValue(this NameValueCollection value,
                                  string? chave)
    {
        if (value is null)
        {
            return null;
        }
        return value[chave];
    }
}
