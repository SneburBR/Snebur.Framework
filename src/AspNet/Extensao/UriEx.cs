namespace Snebur;

#if NET6_0_OR_GREATER == false
public static class UriEx
{
    public static Uri Combine(this Uri baseUri, string relativeUrl)
    {
        if (baseUri == null)
        {
            return null;
        }

        if (String.IsNullOrWhiteSpace(relativeUrl))
        {
            return baseUri;
        }
        return baseUri.Combine(new Uri(relativeUrl, UriKind.RelativeOrAbsolute));
    }

    public static Uri Combine(this Uri baseUri, 
                              Uri relativeUri)
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
#endif
