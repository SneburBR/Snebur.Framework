using System;
using System.Collections.Specialized;
using System.IO;

namespace Snebur.Net
{
    public abstract class SnRequest
    {
        public abstract Stream InputStream { get; }
        public abstract int ContentLength { get; }
        public abstract NameValueCollection Headers { get; }
        public abstract NameValueCollection QueryString { get; }
        public abstract string UserAgent { get; }
        public abstract Uri Url { get; }



    }
}