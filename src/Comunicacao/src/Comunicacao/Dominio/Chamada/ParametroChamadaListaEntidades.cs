namespace Snebur.Comunicacao;

public class ParametroChamadaListaEntidades : ParametroChamadaLista
{
     

    public string? NomeTipoEntidade { get; set; }

    public string? NomeNamespaceNomeTipoEntidade { get; set; }

    public List<Entidade?> Entidades { get; set; } = new();
}
