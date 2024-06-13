using System.Web;
using Snebur.Utilidade;

namespace Snebur.ServicoArquivo
{

//    internal class IpUtilLocal
//    {
//        public static string RetornarIp(HttpContext context)
//        {
//            string ip = null;
//            var httpContext = AplicacaoSnebur.Atual.HttpContext;
//            if (httpContext != null)
//            {
//#if NET6_0_OR_GREATER
//                ip = httpContext.Connection.RemoteIpAddress.ToString();
//#else
//                ip = httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
//                if (!ValidacaoUtil.IsIp(ip))
//                {
//                    ip = httpContext.Request.ServerVariables["REMOTE_ADDR"];
//                }
//#endif

//            }
//            return ip;
//        }
//    }
}