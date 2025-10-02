namespace Snebur.Comunicacao;

public class DominioGlobalizacao : BaseViewModel
{

    #region Campos Privados

    private string? _namespaceGlobalizacao;
    private string? _jsonBase54;

    #endregion

    public DominioGlobalizacao()
    {
    }

    public string? NamespaceGlobalizacao { get => this.GetPropertyValue(this._namespaceGlobalizacao); set => this.SetProperty(this._namespaceGlobalizacao, this._namespaceGlobalizacao = value); }

    public string? JsonBase54 { get => this.GetPropertyValue(this._jsonBase54); set => this.SetProperty(this._jsonBase54, this._jsonBase54 = value); }
}