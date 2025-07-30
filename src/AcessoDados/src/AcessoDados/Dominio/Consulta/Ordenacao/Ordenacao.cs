namespace Snebur.AcessoDados;

public class Ordenacao : BaseAcessoDados
{

    #region Campos Privados

    private string? _caminhoPropriedade;
    private EnumSentidoOrdenacao _sentidoOrdenacaoEnum;

    #endregion

    public string? CaminhoPropriedade { get => this.GetPropertyValue(this._caminhoPropriedade); set => this.SetProperty(this._caminhoPropriedade, this._caminhoPropriedade = value); }

    public EnumSentidoOrdenacao SentidoOrdenacaoEnum { get => this.GetPropertyValue(this._sentidoOrdenacaoEnum); set => this.SetProperty(this._sentidoOrdenacaoEnum, this._sentidoOrdenacaoEnum = value); }
}