namespace Snebur.AcessoDados;

public class ConsultaFiltroColecao : BaseAcessoDados
{
    #region Campos Privados

    private string? _relacao;

    #endregion

    public string? Relacao { get => this.GetPropertyValue(this._relacao); set => this.SetProperty(this._relacao, this._relacao = value); }

    public EstruturaConsulta? EstruturaConsulta { get; set; }
}