using Microsoft.AspNetCore.Http;
using Snebur.AspNetCore.Extensao;
using System;
using System.Collections.Specialized;
using System.IO;

namespace Snebur.AspNetCore.Net.HttpContext
{
    public class SnRequestCore : ZyonRequest
    {
        private NameValueCollection _queryString;
        private NameValueCollection _headers;
        public HttpRequest Request { get; }
        public override Stream InputStream => this.Request.Body;
        public override int ContentLength => (int)this.Request.ContentLength;
        public override NameValueCollection Headers => this._headers;
        public override NameValueCollection QueryString => this._queryString;

        public override string UserAgent => this.Request.UserAgent();

        public override Uri Url => throw new NotImplementedException();


        public ZyonRequestCore(HttpRequest request)
        {
            this.Request = request;

            this._queryString = new NameValueCollection();
            foreach (var chave in request.Query.Keys)
            {
                this._queryString.Add(chave, request.Query[chave].ToString());
            }

            this._headers = new NameValueCollection();
            foreach (var chave in request.Headers.Keys)
            {
                this._headers.Add(chave, request.Headers[chave].ToString());
            }
        }




    }
}