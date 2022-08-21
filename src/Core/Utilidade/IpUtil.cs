using Snebur.Dominio;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Snebur.Utilidade
{
    public static class IpUtil
    {
        private const string IP_VAZIO = "0.0.0.0";
        private const string MASCARA_IP4 = "255.255.255";
        private const string IP_LOCAL = "127.0.0.1";
        private const string IP6_LOCAL = "::1";

        public const string PARAMETRO_IP_REQUISICAO = "IpRequisicao";
        /// <summary>
        /// retornar 0.0.0.0
        /// </summary>
        public static string Empty
        {
            get
            {
                return IP_VAZIO;
            }
        }

        private static DadosIPInformacao _ipDadosInformacao;

        public static DadosIPInformacao RetornarIPInformacao()
        {
            var tipoAplicacao = AplicacaoSnebur.Atual.TipoAplicacao;
            if (tipoAplicacao == EnumTipoAplicacao.DotNet_WebService ||
               tipoAplicacao == EnumTipoAplicacao.Web)
            {
                var ipRequisicao = RetornarIpDaRequisicao();
                return RetornarIPInformacao(ipRequisicao);
            }
            if (_ipDadosInformacao == null)
            {
                _ipDadosInformacao = RetornarIPInformacao(String.Empty);
            }
            return _ipDadosInformacao;
        }
        /// <summary>
        /// Acesso as informações do ip, utilizando no momento um servico gratuito da ipinfo.io
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static DadosIPInformacao RetornarIPInformacao(string ip)
        {
            if (System.Diagnostics.Debugger.IsAttached ||
                ip == "177.128.0.19" ||
                ip == "186.215.185.154")
            {
                return DadosIPInformacao.SneburChapeco;
            }
            if (!String.IsNullOrEmpty(ip) &&
                (ValidacaoUtil.IsIp(ip)))
            {
                if (ip == IP_LOCAL)
                {
                    ip = String.Empty;
                }
            }
            var url = (IpUtil.IsIpVazioOuLocal(ip)) ? "http://ipinfo.io/json" : String.Format("http://ipinfo.io/{0}/json", ip);
            //var cabecalho = RetornarCabecalhoUrlIpInfo();
            var json = HttpUtil.RetornarString(url, null, TimeSpan.FromSeconds(5), true);
            if (!String.IsNullOrWhiteSpace(json))
            {
                var ipInfo = JsonUtil.Deserializar<ipinfo>(json, true);
                var localizacao = Localizacao.Parse(ipInfo.loc);
                var mascaraIp = IpUtil.RetornarMascaraIp4(ipInfo.ip);

                return new DadosIPInformacao
                {
                    IP = ipInfo.ip,
                    Cidade = ipInfo.city,
                    CodigoPostal = ipInfo.postal,
                    Hostname = ipInfo.hostname,
                    Localizacao = localizacao,
                    Regiao = ipInfo.region,
                    Pais = ipInfo.country,
                    ProvedorInternet = ipInfo.org,
                    MascaraIp4 = IpUtil.RetornarMascaraIp4(ipInfo.ip)
                };
            }
            return DadosIPInformacao.Vazio;
        }

        private static string RetornarUrlIpInfo()
        {
            if (AplicacaoSnebur.Atual.TipoAplicacao == EnumTipoAplicacao.DotNet_WebService)
            {
                return "https://ipinfo.snebur.com.br/";
            }
            return "http://ipinfo.io/json";
        }

        private static Dictionary<string, string> RetornarCabecalhoUrlIpInfo()
        {
            if (AplicacaoSnebur.Atual.TipoAplicacao == EnumTipoAplicacao.DotNet_WebService)
            {
                var d = new Dictionary<string, string>();
                throw new NotImplementedException();
            }
            return null;
        }

        public static string RetornarIpInternet()
        {
            var tipoAplicacao = AplicacaoSnebur.Atual.TipoAplicacao;
            switch (tipoAplicacao)
            {
                case (EnumTipoAplicacao.DotNet_WebService):
                    {
                        return RetornarIpDaRequisicao();
                    }
                case (EnumTipoAplicacao.DotNet_Wpf):
                case (EnumTipoAplicacao.DotNet_WindowService):
                case (EnumTipoAplicacao.DotNet_UnitTest):

                    return IpUtil.RetornarIPInformacao().IP;

                default:
                    throw new ErroNaoSuportado("O tipo de aplicação não é suportado");
            }
        }

        public static string RetornarMascaraIp4(string ip)
        {
            IPAddress ipaddress;
            if (IPAddress.TryParse(ip, out ipaddress))
            {
                if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    var partes = ipaddress.ToString().Split('.');
                    if (byte.TryParse(partes[0], out byte parte1) &&
                        byte.TryParse(partes[1], out byte parte2) &&
                        byte.TryParse(partes[2], out byte parte3))
                    {
                        var mascara = String.Format("{0}.{1}.{2}", parte1.ToString(), parte2.ToString(), parte3.ToString());
                        return mascara;
                    }
                }
            }
            return null;
        }

        public static bool IsIP(string ip)
        {
            return ValidacaoUtil.IsIp(ip);
        }

        public static bool IsIpVazioOuLocal(string ip)
        {
            if (String.IsNullOrEmpty(ip))
            {
                return true;
            }
            if (ip == IP_LOCAL || ip == IP6_LOCAL || ip == IP_VAZIO)
            {
                return true;
            }
            return false;
        }

        private static Localizacao RetornarLocalizacao(string localicacaoString)
        {
            var partes = localicacaoString.Split(',');
            if (partes.Length != 2)
            {
                return Localizacao.Empty;
            }
            var latituraString = partes.First();
            var longitudeString = partes.Last();

            if (Double.TryParse(latituraString, out double latitude) &&
                Double.TryParse(longitudeString, out double longitude))
            {
                return new Localizacao(latitude, longitude);
            }
            return new Localizacao(0, 0);
        }

        public static string RetornarIpDaRequisicao(bool isRetornarNullNaoEncotnrado = true)
        {
            var httpContext = AplicacaoSnebur.Atual.HttpContext;

            string ip = null;
            if (httpContext != null)
            {
#if NetCore
                ip = httpContext.Connection.RemoteIpAddress.ToString();
#else
                if (httpContext.Request.Headers[PARAMETRO_IP_REQUISICAO] != null)
                {
                    var ipRequisicao = httpContext.Request.Headers[PARAMETRO_IP_REQUISICAO];
                    if (IpUtil.IsIP(ipRequisicao))
                    {
                        return ipRequisicao;
                    }
                }
                ip = httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!ValidacaoUtil.IsIp(ip))
                {
                    ip = httpContext.Request.ServerVariables["REMOTE_ADDR"];
                }
#endif
                if (ip == IP6_LOCAL)
                {
                    return IP_LOCAL;
                }
                if (ValidacaoUtil.IsIp(ip))
                {
                    return ip;
                }
            }
            if (isRetornarNullNaoEncotnrado)
            {
                return null;
            }
            throw new Exception("O ip não foi encontrado");
        }

        public static string RetornarIpLocal(bool isIgnorarErro = false)
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            if (isIgnorarErro)
            {
                return null;
            }
            throw new Exception("O rede local não existe ou está desativada  ");
        }

        private class ipinfo
        {
            public string ip { get; set; }
            public string hostname { get; set; }
            public string city { get; set; }
            public string region { get; set; }
            public string country { get; set; }
            public string loc { get; set; }
            public string org { get; set; }
            public string postal { get; set; }
        }
    }
}
namespace Snebur.Dominio
{
    public class DadosIPInformacao : BaseDominio, IIPInformacao
    {
        public string IP { get; set; }

        public string MascaraIp4 { get; set; }

        public string Cidade { get; set; }

        public string CodigoPostal { get; set; }

        public string Hostname { get; set; }

        public Localizacao Localizacao { get; set; }

        public string Pais { get; set; }

        public string ProvedorInternet { get; set; }

        public string Regiao { get; set; }

        public static DadosIPInformacao Vazio
        {
            get
            {
                return new DadosIPInformacao
                {
                    IP = IpUtil.Empty
                };
            }
        }

        internal static DadosIPInformacao SneburChapeco
        {
            get
            {
                var ipLocal = "177.128.0.19";
                return new DadosIPInformacao
                {
                    IP = ipLocal,
                    Hostname = "CHA-SNEBUR",
                    Regiao = "Santa Catarina",
                    Pais = "BR",
                    Cidade = "Chapecó",
                    Localizacao = new Localizacao(-27.1000, -52.6000),
                    ProvedorInternet = "AS262361 Deznet Telecom Ltda.",
                    MascaraIp4 = IpUtil.RetornarMascaraIp4(ipLocal)
                };
            }
        }
    }
}