﻿using Snebur.Net;
using Snebur.Utilidade;

namespace Snebur.ServicoArquivo
{

    internal class IpUtilLocal
    {
        public static string RetornarIp(ZyonHttpContext context)
        {
            string ip = null;
            var httpContext = AplicacaoSnebur.Atual.HttpContext;
            if (httpContext != null)
            {
#if NET7_0
                ip = httpContext.Connection.RemoteIpAddress.ToString();
#else
                    ip = httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!ValidacaoUtil.IsIp(ip))
                {
                    ip = httpContext.Request.ServerVariables["REMOTE_ADDR"];
                }
#endif

            }
            return ip;
        }
    }
}