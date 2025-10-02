namespace Snebur.Comunicacao;

public class ParametroChamadaEnum : ParametroChamada
{
    #region Campos Privados

    private string? _nomeTipoEnum;
    private string? _namespaceEnum;
    private int _valor;

    #endregion

    public string? NomeTipoEnum { get => this.GetPropertyValue(this._nomeTipoEnum); set => this.SetProperty(this._nomeTipoEnum, this._nomeTipoEnum = value); }

    public string? NamespaceEnum { get => this.GetPropertyValue(this._namespaceEnum); set => this.SetProperty(this._namespaceEnum, this._namespaceEnum = value); }

    public int Valor { get => this.GetPropertyValue(this._valor); set => this.SetProperty(this._valor, this._valor = value); }
}