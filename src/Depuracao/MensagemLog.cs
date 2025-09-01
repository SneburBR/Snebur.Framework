namespace Snebur.Depuracao;

public class MensagemLog : Mensagem
{

    #region Campos Privados

    private string _mensagem;
    private EnumTipoLog _tipoLog;

    public MensagemLog(string mensagem, EnumTipoLog tipoLog)
    {
        this._mensagem = mensagem;
        this._tipoLog = tipoLog;
    }

    #endregion

    public string Mensagem { get => this.GetPropertyValue(this._mensagem); set => this.SetProperty(this._mensagem, this._mensagem = value); }

    public EnumTipoLog TipoLog { get => this.GetPropertyValue(this._tipoLog); set => this.SetProperty(this._tipoLog, this._tipoLog = value); }
}

public enum EnumTipoLog
{
    Normal,
    Alerta,
    Erro,
    Sucesso,
    Acao
}