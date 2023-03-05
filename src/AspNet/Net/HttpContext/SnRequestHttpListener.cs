using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;

namespace Snebur.Net
{
    internal class SnRequestHttpListener : SnRequest
    {
        private HttpListenerRequest Request { get; }
        private NameValueCollection _queryString = null;

        public SnRequestHttpListener(HttpListenerRequest request)
        {
            this.Request = request;
        }

        public override Stream InputStream => this.Request.InputStream;

        public override int ContentLength => (int)this.Request.ContentLength64;

        public override NameValueCollection Headers => this.Request.Headers;

        public override Uri Url => this.Request.Url;

        public override NameValueCollection QueryString
        {
            get
            {
                if (this._queryString == null)
                {
                    this._queryString = this.RetornarQueryString();
                }
                return this._queryString;
            }
        }

        public override string UserAgent => this.Request.UserAgent;

        private NameValueCollection RetornarQueryString()
        {
            var retorno = new NameValueCollection();
            var query = this.Request.Url.Query.Trim();
            if (query.Length > 0)
            {
                query = query.Substring(1);
                var partes = query.Split('&');
                foreach (var parte in partes)
                {
                    var pares = parte.Split('=');
                    if (pares.Length > 2)
                    {
                        throw new Erro($"A parte '{parte}' não é par de chave e valor");
                    }
                    var chave = pares[0];
                    var valor = (pares.Length > 1) ? pares[1] : String.Empty;
                    chave = HttpUtility.UrlDecode(chave);
                    valor = HttpUtility.UrlDecode(valor);
                    retorno.Add(chave, valor);
                }
            }
            return retorno;
        }
    }
}