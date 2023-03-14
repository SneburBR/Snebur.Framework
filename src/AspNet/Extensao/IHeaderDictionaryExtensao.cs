#if NET7_0
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Net.Http.Headers;
#endif

namespace System.Web
{
    public static class IHeaderDictionaryExtensao
    {
#if NET7_0
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

        public static string[] GetValues(this IHeaderDictionary cabecalho, string chave)
        {
            if (cabecalho.TryGetValue(chave, out var item))
            {
                return item.ToArray();
            }
            return null;
        }
#endif
    }

}
