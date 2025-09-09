namespace Snebur.AcessoDados.Mapeamento;

internal class GrupoIdTipoEntidade
{
    public SortedSet<long> Ids { get; set; }

    public string NomeTipoEntidade { get; set; }

    public GrupoIdTipoEntidade(SortedSet<long> ids, string nomeTipoEntidade)
    {
        this.Ids = ids;
        this.NomeTipoEntidade = nomeTipoEntidade;
    }
}