using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Snebur.Utilidade
{
    public static class UriUtil
    {
        public static Uri RetornarUriParent(Uri uri)
        {
            char[] divisor = @"/".ToCharArray();

            string[] seguementos = uri.AbsolutePath.Split(divisor);
            List<string> paths = new List<string>();
            for (int i = 0; i <= seguementos.Length - 2; i++)
            {
                paths.Add(seguementos[i]);
            }
            string path = String.Join(@"/", paths);
            UriBuilder uriBuilder = new UriBuilder(uri.Scheme, uri.Host, uri.Port, path);
            return uriBuilder.Uri;
        }


        public static string CombinarCaminhos(string endereco1, string endereco2)
        {
            return UriUtil.AjustarBarraFinal(endereco1) + UriUtil.RemoverBarraInicial(endereco2);
        }

        public static string AjustarBarraInicial(string endereco)
        {
            endereco = UriUtil.InverterBarras(endereco);
            if (!endereco.StartsWith("/"))
            {
                return "/" + endereco;
            }
            else
            {
                return endereco;
            }
        }

        public static string RetornarURL(string urlVisualizar, Dictionary<string, string> parametros)
        {
            var query = String.Join("&", parametros.Select(x => $"{HttpUtility.UrlEncode(Base64Util.Encode(x.Key))}={HttpUtility.UrlEncode(Base64Util.Encode(x.Value))}"));
            return AjustarInterrogacaoFunal(urlVisualizar) + query;
        }

        private static string AjustarInterrogacaoFunal(string urlVisualizar)
        {
            if (!urlVisualizar.EndsWith("?"))
            {
                return urlVisualizar + "?";
            }
            return urlVisualizar;
        }

        public static string AjustarBarraFinal(string endereco)
        {
            endereco = UriUtil.InverterBarras(endereco);
            if (!endereco.EndsWith("/"))
            {
                return endereco + "/";
            }
            else
            {
                return endereco;
            }
        }

        public static string RemoverBarrasFinalInicial(string endereco)
        {
            return UriUtil.RemoverBarraInicial(RemoverBarraFinal(endereco));
        }
        public static string RemoverBarraInicial(string endereco)
        {
            endereco = UriUtil.InverterBarras(endereco);
            while (endereco.StartsWith("/"))
            {
                endereco = endereco.Remove(0, 1);
            }
            return endereco;
        }

        public static string RemoverBarraFinal(string endereco)
        {
            endereco = UriUtil.InverterBarras(endereco);
            while (endereco.EndsWith("/"))
            {
                endereco = endereco.Remove(endereco.Length - 1, 1);
            }
            return endereco;
        }

        private static string InverterBarras(string endereco)
        {
            return endereco.Replace(@"\", "/");
        }

        public static Dictionary<string, string> RetornarPartesConsulta(Uri uri)
        {
            return RetornarPartesConsulta(uri.Query);
        }

        public static Dictionary<string, string> RetornarPartesConsulta(string query)
        {
            var retorno = new Dictionary<string, string>();

            if (query.Trim().Length > 0)
            {
                if (query.StartsWith("?"))
                {
                    query = query.Substring(1);
                }
                string[] partes = query.Split("&".ToCharArray());
                foreach (string parte in partes)
                {
                    if (parte.Contains("="))
                    {
                        string[] valores = parte.Split("=".ToCharArray());
                        string chave = valores.First();
                        string valor = parte.Replace(String.Format("{0}=", chave), String.Empty);

                        if (retorno.ContainsKey(chave))
                        {
                            retorno[chave] += "," + HttpUtility.UrlDecode(valor);
                        }
                        else
                        {
                            retorno.Add(chave, HttpUtility.UrlDecode(valor));
                        }
                    }
                }
            }
            return retorno;
        }

        public static string ConstruirQuery(NameValueCollection parametros)
        {
            return ConstruirQuery(parametros.ToDictionary());
        }
        public static string ConstruirQuery(IDictionary<string, string> di)
        {
            return String.Join("&", di.Select(x => String.Format("{0}={1}", x.Key, Uri.EscapeUriString(x.Value))));
        }

        public static string RetornarValorQuery(string query, string chave)
        {
            var partes = RetornarPartesConsulta(query);
            if (partes.ContainsKey(chave))
            {
                return partes[chave];
            }
            return null;
        }
    }
}