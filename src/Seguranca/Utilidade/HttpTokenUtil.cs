using Snebur.Seguranca;
using System;
using System.Collections.Generic;

#if NET50 == false
using System.Web;
#endif 

namespace Snebur.Utilidade
{
    public class HttpTokenUtil
    {
        public static T RetornarResultado<T>(string url,
                                             Dictionary<string, string> parametros = null,
                                             TimeSpan? timeout = null,
                                             bool isIgnorarErro = false)
        {
            if (parametros == null)
            {
                parametros = new Dictionary<string, string>();
            }

            if (!parametros.ContainsKey(ConstantesCabecalho.TOKEN))
            {
                var token = Token.RetornarToken();
                parametros.Add(ConstantesCabecalho.TOKEN, HttpUtility.UrlEncode(token));
            }

            if (!parametros.ContainsKey(ConstantesCabecalho.NOME_ASSEMBLY_APLICACAO))
            {
                parametros.Add(ConstantesCabecalho.NOME_ASSEMBLY_APLICACAO,
                               HttpUtility.UrlEncode(AplicacaoSnebur.Atual.NomeAplicacao));
            }

            var timeoutTipado = timeout ?? TimeSpan.FromSeconds(HttpUtil.TIMEOUT_PADRAO);
            var json = HttpUtil.RetornarString(url,
                                               parametros,
                                               timeoutTipado,
                                               isIgnorarErro);

            try
            {
                return JsonUtil.Deserializar<T>(json, false);

            }
            catch (Exception ex)
            {
                LogUtil.ErroAsync(ex);

                if (isIgnorarErro)
                {
                    return default;
                }
                throw ex;
            }
        }
    }
}