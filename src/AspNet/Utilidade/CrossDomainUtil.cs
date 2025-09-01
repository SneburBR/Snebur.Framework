using System.Web;

#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Http;
#else
//using System.Web;
#endif 

namespace Snebur.Utilidade;

public static class CrossDomainUtil
{
    private const string ACCESS_CONTROL_REQUEST_METHOD = "Access-Control-Request-Method";
    private const string ACCESS_CONTROL_REQUEST_HEADERS = "Access-Control-Request-Headers";
    private const string ORIGIN = "Origin";

    private const string ACCESS_CONTROL_ALLOW_ORIGIN = "Access-Control-Allow-Origin";
    private const string ACCESS_CONTROL_ALLOW_METHODS = "Access-Control-Allow-Methods";
    private const string ACCESS_CONTROL_ALLOW_HEADERS = "Access-Control-Allow-Headers";

    public static bool VerificarContratoCrossDomain(HttpContext context)
    {
        var resposta = context.Response;
        resposta.Headers.Append(ACCESS_CONTROL_ALLOW_ORIGIN, "*");
        var requisicao = context.Request;
        var method = requisicao.GetMethod();
        if(method != "OPTIONS")
        {
            return false;
        }

        //var origens = requisicao.Headers.GetValues(ORIGIN);

        //if (origens != null && origens.Length > 0)
        //{
        //    resposta.Headers.Append(ACCESS_CONTROL_ALLOW_ORIGIN, String.Join(", ", origens));
        //}
        //else
        //{
        //    resposta.Headers.Append(ACCESS_CONTROL_ALLOW_ORIGIN, "*");
        //}
     

        var metodosRequisicao = requisicao.Headers.GetValues(ACCESS_CONTROL_REQUEST_METHOD);
        var cabecalhosRequisicao = requisicao.Headers.GetValues(ACCESS_CONTROL_REQUEST_HEADERS);
         
        if (metodosRequisicao != null)
        {
            resposta.Headers.Append(ACCESS_CONTROL_ALLOW_METHODS, String.Join(", ", metodosRequisicao));
        }

        if (cabecalhosRequisicao != null)
        {
            resposta.Headers.Append(ACCESS_CONTROL_ALLOW_HEADERS, String.Join(", ", cabecalhosRequisicao));
        }
        return true;
    }
}