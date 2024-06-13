#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Http;
#else
using System.Collections;
#endif
namespace Snebur
{
    public static class IDictionaryExtensao
    {
#if NET6_0_OR_GREATER == false

        public static bool ContainsKey(this IDictionary dictionary, string chave)
        {
            return dictionary.Contains(chave);
        }

#endif
    }
}