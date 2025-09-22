namespace Snebur.Comunicacao.Dominio;

public class ResultadoGlobalizacao : BaseViewModel
{
    #region Campos Privados

    private string? _jsonIdiomaBase64;
    private string? _jsonCulturaBase64;

    #endregion

    public string? JsonIdiomaBase64 { get => this.GetPropertyValue(this._jsonIdiomaBase64); set => this.SetProperty(this._jsonIdiomaBase64, this._jsonIdiomaBase64 = value); }

    public string? JsonCulturaBase64 { get => this.GetPropertyValue(this._jsonCulturaBase64); set => this.SetProperty(this._jsonCulturaBase64, this._jsonCulturaBase64 = value); }

    public List<DominioGlobalizacao> Dominios { get; set; } = new();

    public List<TelaGlobalizacao> Telas { get; set; } = new();
}