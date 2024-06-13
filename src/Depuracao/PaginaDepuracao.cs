#if NET48

using System;
using System.Linq;
using static Snebur.Depuracao.ConstantesDeburacao;

namespace Snebur.Depuracao
{
    public abstract class PaginaDepuracao : System.Web.UI.Page
    {

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            var url = this.Request.RawUrl;
            if (!this.Request.QueryString.AllKeys.Contains(CHAVE_GUID))
            {
                if (!url.Contains("?"))
                {
                    url += "?";
                }
                else
                {
                    url += "&";
                }

                url += $"{CHAVE_GUID}={Guid.NewGuid().ToString().Split('-').First()}";
            }

            if (!this.Request.RawUrl.Contains(PARAMETRO_VS_DEPURACAO_PORTA) &&
                (System.Diagnostics.Debugger.IsAttached || this.IsDepurar))
            {
                var diretorio = this.Server.MapPath("/");
                var urlDepuracao = DepuracaoUtil.RetornarUrlDepuracao(diretorio, url);
                if (urlDepuracao != null)
                {
                    this.Response.Redirect(urlDepuracao, false);
                    return;
                }
            }
            base.OnLoad(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private bool IsDepurar => true;
        //Convert.ToBoolean(this.Request.QueryString[PARAMETRO_DEPURAR]);
            
    }
}
#endif