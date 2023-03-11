#if NET7_0
using Microsoft.AspNetCore.Http;
#else
using System.Collections;
using System.Web;
#endif
namespace Snebur
{
   
    public static class HttpContextExtensao
    {
        public static void AdicionrItem(this HttpContext context, string chave, object item)
        {
            if (!context.Items.ContainsKey(chave))
            {
                context.Items.Add(chave, item);
            }
        }
        public static void RemoverItem(this HttpContext context, string chave)
        {
            if (context.Items.ContainsKey(chave))
            {
                context.Items.Remove(chave);
            }
        }

#if NET7_0 == false

        public static bool ContainsKey(this IDictionary dictionary, string chave)
        {
            return dictionary.Contains(chave);
        }

#endif

    }
}