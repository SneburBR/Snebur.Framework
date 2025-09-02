namespace Snebur.Comunicacao.Dominio;

public abstract class ResultadoChamada : BaseComunicao
{

    #region Campos Privados

    private string? _nomeServico;
    private DateTime _dataHora;
    private string? _operacao;

    #endregion

    public string? NomeServico { get => this.GetPropertyValue(this._nomeServico); set => this.SetProperty(this._nomeServico, this._nomeServico = value); }
    public DateTime DataHora { get => this.GetPropertyValue(this._dataHora); set => this.SetProperty(this._dataHora, this._dataHora = value); }
    public string? Operacao { get => this.GetPropertyValue(this._operacao); set => this.SetProperty(this._operacao, this._operacao = value); }
    public BaseDominio? ExtraOpcional { get; set; }
}