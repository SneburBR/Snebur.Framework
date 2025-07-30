namespace Snebur.AcessoDados;

public class PropriedadeComputada : BaseAcessoDados
{
    #region Campos Privados

    private string? _nomePropriedade;
    private object? _valor;

    #endregion

    public string? NomePropriedade { get => this.GetPropertyValue(this._nomePropriedade); set => this.SetProperty(this._nomePropriedade, this._nomePropriedade = value); }

    public object? Valor { get => this.GetPropertyValue(this._valor); set => this.SetProperty(this._valor, this._valor = value); }
}