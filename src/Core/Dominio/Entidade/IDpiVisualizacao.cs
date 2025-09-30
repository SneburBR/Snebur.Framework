namespace Snebur.Dominio;

public interface IDpiVisualizacao
{
    [IgnorarPropriedade]
    Func<double?, double>? FuncaoNormamlizarDpiVisualizacao { get; set; }
}
