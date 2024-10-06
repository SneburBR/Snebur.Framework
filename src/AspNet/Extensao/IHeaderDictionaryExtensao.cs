#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Net.Http.Headers;
#else
using System.Collections.Specialized;
#endif



namespace System.Web
{
    public static class IHeaderDictionaryExtensao
    {

#if NET6_0_OR_GREATER

        public static string GetValue(this IHeaderDictionary cabecalho, string chave)
        {
            if (cabecalho.TryGetValue(chave, out var item))
            {
                if (item.Count == 1)
                {
                    return item.ToString();
                }
                throw new Erro($"Existe mais de um item no cabeçalho da requisição para chave {chave}");
            }
            return null;
        }

        public static void Add(this IHeaderDictionary cabecalho, string chave, long value)
        {
            cabecalho.Append(chave, value.ToString());
        }
        public static void Add(this IHeaderDictionary cabecalho, string chave, int value)
        {
            cabecalho.Append(chave, value.ToString());
        }

        public static string[] GetValues(this IHeaderDictionary cabecalho, string chave)
        {
            if (cabecalho.TryGetValue(chave, out var item))
            {
                return item.ToArray();
            }
            return null;
        }
#else

        public static void Append(this NameValueCollection nameValueCollection,
                                  string key, 
                                  string value)
        {
            nameValueCollection.Add(key, value);
        }
#endif
    }

    
}
