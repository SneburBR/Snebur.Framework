namespace Snebur.Depuracao;

public class MensagemPing : Mensagem
{

    #region Campos Privados

    private bool _ping;
    private DateTime _dataHora;

    public MensagemPing(bool ping, DateTime dataHora)
    {
        this._ping = ping;
        this._dataHora = dataHora;
    }

    #endregion

    public bool Ping { get => this.GetPropertyValue(this._ping); set => this.SetProperty(this._ping, this._ping = value); }

    public DateTime DataHora { get => this.GetPropertyValue(this._dataHora); set => this.SetProperty(this._dataHora, this._dataHora = value); }
}