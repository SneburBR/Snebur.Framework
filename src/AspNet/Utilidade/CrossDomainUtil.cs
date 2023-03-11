
using System;

#if NET7_0 
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif 

namespace Snebur.Utilidade
{
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
            var requisicao = context.Request;
            var resposta = context.Response;

            var origens = requisicao.Headers.GetValues(ORIGIN);
            if (origens != null && origens.Length > 0)
            {
                resposta.Headers.Add(ACCESS_CONTROL_ALLOW_ORIGIN, String.Join(", ", origens));
            }
            else
            {
                resposta.Headers.Add(ACCESS_CONTROL_ALLOW_ORIGIN, "*");
            }
            var metodosRequisicao = requisicao.Headers.GetValues(ACCESS_CONTROL_REQUEST_METHOD);
            var cabecalhosRequisicao = requisicao.Headers.GetValues(ACCESS_CONTROL_REQUEST_HEADERS);

            if (metodosRequisicao == null && cabecalhosRequisicao == null)
            {
                return false;
            }
            if (metodosRequisicao != null)
            {
                resposta.Headers.Add(ACCESS_CONTROL_ALLOW_METHODS, String.Join(", ", metodosRequisicao));
            }
            if (cabecalhosRequisicao != null)
            {
                resposta.Headers.Add(ACCESS_CONTROL_ALLOW_HEADERS, String.Join(", ", cabecalhosRequisicao));
            }
            return true;
        }
    }
}