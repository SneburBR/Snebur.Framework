using Snebur.Utilidade;
using System.Web;

namespace System
{
    public static class UriExtensao
    {
        public static string GetQuery(this Uri uri)
        {
            if (uri.IsAbsoluteUri)
            {
                return uri.Query;
            }
            var url = uri.ToString();
            if (url.Contains("?"))
            {
                return url.Substring(url.IndexOf("?") + 1);
            }
            return String.Empty;
        }

        public static string GetPath(this Uri uri, bool isRemoveFileName)
        {
            var path = uri.GetPath();
            if (String.IsNullOrWhiteSpace(path))
            {
                return path;
            }

            if (isRemoveFileName)
            {
                var fileName = IO.Path.GetFileName(path);
                if (fileName.Contains("."))
                {
                    return IO.Path.GetDirectoryName(path);
                }
            }
            return path;

        }
        public static string GetPath(this Uri uri)
        {
            if (uri.IsAbsoluteUri)
            {
                return uri.AbsolutePath;
            }

            var url = uri.ToString();
            if (url.Contains("?"))
            {
                return url.Substring(0, url.IndexOf("?"));
            }
            if (url.Contains("#"))
            {
                return url.Substring(0, url.IndexOf("#"));
            }
            return uri.ToString();
        }

        public static Uri Combine(this Uri baseUri, string relativeUrl)
        {
            return baseUri.Combine(new Uri(relativeUrl, UriKind.RelativeOrAbsolute));
        }

        public static Uri Combine(this Uri baseUri, Uri relativeUri)
        {
            if (baseUri == null) throw new ArgumentNullException("baseUri");
            if (relativeUri == null) throw new ArgumentNullException("relativeUri");

            if (relativeUri.IsAbsoluteUri)
            {
                return relativeUri;
            }

            var query = HttpUtility.ParseQueryString(baseUri.Query);
            var queryRelativa = HttpUtility.ParseQueryString(relativeUri.GetQuery());
            query.AddRange(queryRelativa);


            var basePath = baseUri.GetPath(true);
            var relativePath = relativeUri.GetPath();

            var path = String.IsNullOrEmpty(relativePath) ? basePath :
                                                            VirtualPathUtility.Combine(basePath, relativePath);

            var uriBuilder = new UriBuilder(baseUri.ToString())
            {
                Path = path,
                Query = UriUtil.ConstruirQuery(query)
            };
            return uriBuilder.Uri;
        }
    }
}
