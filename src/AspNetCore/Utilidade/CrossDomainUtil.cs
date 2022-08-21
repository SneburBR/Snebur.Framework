using System;
using Microsoft.AspNetCore.Http;

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
            var origem = (origens != null && origens.Length > 0) ? String.Join(", ", origens) : "*";

            if (resposta.Headers.ContainsKey(ACCESS_CONTROL_ALLOW_ORIGIN))
            {
                resposta.Headers[ACCESS_CONTROL_ALLOW_ORIGIN] = origem;
            }
            else
            {
                resposta.Headers.Add(ACCESS_CONTROL_ALLOW_ORIGIN, origem);
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
