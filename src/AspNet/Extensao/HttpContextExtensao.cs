#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Http;
using System.IO;
#else
using System.Web;
#endif
namespace Snebur;


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

    public static string GetMapPath(this HttpContext context, string path)
    {
#if NET6_0_OR_GREATER
        if (context.Items.TryGetValue(ConstantesItensRequsicao.CAMINHO_APLICACAO, out var caminhoAplicacao) &&
            caminhoAplicacao is string caminhoAplicacaoTipado)
        {
            Guard.NotNullOrWhiteSpace(caminhoAplicacaoTipado);

            return Path.Combine(caminhoAplicacaoTipado, path);
        }
        throw new DirectoryNotFoundException("Caminho da aplicação  foi definido no HttpContext");

#else
        return context.Server.MapPath(path);
#endif
    }

}