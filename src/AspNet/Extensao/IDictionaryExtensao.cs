#if NET7_0
using Microsoft.AspNetCore.Http;
#else
using System.Collections;
#endif
namespace Snebur
{
    public static class IDictionaryExtensao
    {
#if NET7_0 == false

        public static bool ContainsKey(this IDictionary dictionary, string chave)
        {
            return dictionary.Contains(chave);
        }

#endif
    }
}