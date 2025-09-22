namespace Snebur.AcessoDados;

public abstract class BaseFiltroGrupo : BaseFiltro
{
    #region Campos Privados

    #endregion

    public List<BaseFiltro> Filtros { get; set; } = new();
}