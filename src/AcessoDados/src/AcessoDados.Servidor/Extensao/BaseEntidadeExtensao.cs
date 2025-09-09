namespace Snebur.Dominio;

public static class BaseEntidadeExtensao
{
    public static string RetornarCaminhoTipo(this Entidade baseEntidade)
    {
        var tipo = baseEntidade.GetType();
        return String.Format("{0}.{1}", tipo.Namespace, tipo.Name);
    }
}
