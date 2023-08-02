namespace System.Collections.Specialized
{
    public static class NameValueCollectionExtensao
    {
        public static string GetValue(this NameValueCollection value,
                                      string chave)
        {
            return value[chave];
        }
    }
}
