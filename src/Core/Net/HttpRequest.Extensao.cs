namespace System.Web
{
    public static class HttpRequestExtensao
    {
        //Para compatibilidade do .Net  5.0
        public static Uri RetornarUrlRequisicao(this HttpRequest httpRequest)
        {
            return httpRequest.UrlReferrer ??
                   httpRequest.Url;
        }
    }
}
