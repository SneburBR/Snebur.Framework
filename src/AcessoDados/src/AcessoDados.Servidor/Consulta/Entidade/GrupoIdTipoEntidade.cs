namespace Snebur.AcessoDados.Mapeamento;

internal class IdTipoEntidade : INomeTipoEntidade
{

    public long Id { get; set; }

    public string? __NomeTipoEntidade { get; set; }

    public long CampoFiltro { get; set; }

    //string INomeTipoEntidade.__NomeTipoEntidade => throw new NotImplementedException();
}
