using System;
using System.Collections.Specialized;
using System.IO;


#if NetCore
using Microsoft.AspNetCore.Http;
using Snebur.Utilidade;
#else
using System.Web;
#endif

namespace Snebur.Net
{
    public class SnRequestHttp : SnRequest
    {

#if NetCore
        private NameValueCollection _headers;
        private NameValueCollection _queryString;
#endif
        public HttpRequest Request { get; }

        public SnRequestHttp(HttpRequest request)
        {
            this.Request = request;
        }

        public override Stream InputStream
        {
            get
            {
#if NetCore
                return this.Request.Body;
#else
                return this.Request.InputStream;
#endif
            }
        }

        public override int ContentLength
        {
            get
            {
#if NetCore
                return Convert.ToInt32(this.Request.ContentLength);
#else
                return this.Request.ContentLength;
#endif
            }
        }


        public override NameValueCollection Headers
        {
            get
            {
#if NetCore
                return ThreadUtil.RetornarValorComBloqueio(ref this._headers, this.RetornarHearder);
#else
                return this.Request.Headers;
#endif
            }
        }

        //public override NameValueCollection ServerVariables => this.Request.ServerVariables;

        public override NameValueCollection QueryString
        {
            get
            {
#if NetCore
                return ThreadUtil.RetornarValorComBloqueio(ref this._queryString, this.RetornarQueryString);
#else
                return this.Request.QueryString;
#endif
            }
        }

        public override string UserAgent
        {
            get
            {
#if NetCore
                throw new NotImplementedException();
#else
                return this.Request.UserAgent;
#endif
            }
        } 

        public override Uri Url {

            get
            {
#if NetCore
                throw new NotImplementedException();
#else
                return this.Request.Url;
#endif
            }
        }

#if NetCore
        private NameValueCollection RetornarHearder()
        {
            var headers = new NameValueCollection();
            foreach (var x in this.Request.Headers)
            {
                headers.Add(x.Key, x.Value.ToString());
            }
            return headers;
        }

        private NameValueCollection RetornarQueryString()
        {
            var queryString = new NameValueCollection();
            foreach (var x in this.Request.Query)
            {
                queryString.Add(x.Key, x.Value.ToString());
            }
            return queryString;
        }

#endif
    }
}
