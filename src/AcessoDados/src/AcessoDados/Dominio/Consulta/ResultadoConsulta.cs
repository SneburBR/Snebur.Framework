namespace Snebur.AcessoDados;

public class ResultadoConsulta : Resultado
{
    #region Campos Privados

    private int _totalRegistros;

    #endregion

    public ListaEntidades<IEntidade> Entidades { get; set; } = new ListaEntidades<IEntidade>();

    public int TotalRegistros { get => this.GetPropertyValue(this._totalRegistros); set => this.SetProperty(this._totalRegistros, this._totalRegistros = value); }

    //public PaginacaoConsulta PaginacaoConsulta { get; set; }
}
