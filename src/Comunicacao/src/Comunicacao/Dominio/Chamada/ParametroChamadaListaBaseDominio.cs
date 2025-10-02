namespace Snebur.Comunicacao;

public class ParametroChamadaListaBaseDominio : ParametroChamadaLista
{
    #region Campos Privados

    private string? _nomeTipoBaseDominio;
    private string? _nomeNamespaceTipoBaseDominio;

    #endregion

    public string? NomeTipoBaseDominio { get => this.GetPropertyValue(this._nomeTipoBaseDominio); set => this.SetProperty(this._nomeTipoBaseDominio, this._nomeTipoBaseDominio = value); }

    public string? NomeNamespaceTipoBaseDominio { get => this.GetPropertyValue(this._nomeNamespaceTipoBaseDominio); set => this.SetProperty(this._nomeNamespaceTipoBaseDominio, this._nomeNamespaceTipoBaseDominio = value); }

    public List<BaseDominio?> BasesDominio { get; set; } = new();
}