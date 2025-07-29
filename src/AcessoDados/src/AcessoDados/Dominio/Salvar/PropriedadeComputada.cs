namespace Snebur.AcessoDados;

public class PropriedadeComputada : BaseAcessoDados
{
    #region Campos Privados

    private string? _nomePropriedade;
    private object? _valor;

    #endregion

    public string? NomePropriedade { get => this.RetornarValorPropriedade(this._nomePropriedade); set => this.NotificarValorPropriedadeAlterada(this._nomePropriedade, this._nomePropriedade = value); }

    public object? Valor { get => this.RetornarValorPropriedade(this._valor); set => this.NotificarValorPropriedadeAlterada(this._valor, this._valor = value); }
}