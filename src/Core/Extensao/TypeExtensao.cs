namespace System
{
    public static class TypeExtensao
    {
        public static string RetornarCaminhoTipo(this Type tipo)
        {
            return $"{tipo.Namespace}.{tipo.Name}";
        }
    }
}
