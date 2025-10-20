namespace Snebur.Comunicacao;

public class ParametroChamadaListaTipoComplexo : ParametroChamadaLista
{
    public string? NomeTipoComplexo { get; set; }
    public string? NomeNamespaceTipoComplexo { get; set; }
    public List<BaseTipoComplexo?> TiposComplexo { get; set; } = new();
}