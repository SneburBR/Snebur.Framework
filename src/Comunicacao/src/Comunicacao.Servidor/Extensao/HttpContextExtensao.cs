using System.Collections;
using System.Web;



#if NetCore
using Microsoft.AspNetCore.Http;
using Snebur.AspNetCore;
#else
#endif

namespace Snebur.Comunicacao
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

#if NetCore == false
        public static bool ContainsKey(this IDictionary dictionary, string chave)
        {
            return dictionary.Contains(chave);
        }
#endif

    }
}