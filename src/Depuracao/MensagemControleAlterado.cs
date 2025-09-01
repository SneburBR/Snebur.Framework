namespace Snebur.Depuracao;

public class MensagemControleAlterado : Mensagem
{

    #region Campos Privados

    private bool _isScript;
    private string _urlScriptRuntime;
    private string _caminhoConstrutor;
    private string _nomeControle;

    public MensagemControleAlterado(
        bool isScript,
        string urlScriptRuntime,
        string caminhoConstrutor,
        string nomeControle)
    {
        this._isScript = isScript;
        this._urlScriptRuntime = urlScriptRuntime;
        this._caminhoConstrutor = caminhoConstrutor;
        this._nomeControle = nomeControle;
    }

    #endregion

    public bool IsScript { get => this.GetPropertyValue(this._isScript); set => this.SetProperty(this._isScript, this._isScript = value); }

    public string UrlScriptRuntime { get => this.GetPropertyValue(this._urlScriptRuntime); set => this.SetProperty(this._urlScriptRuntime, this._urlScriptRuntime = value); }

    public string CaminhoConstrutor { get => this.GetPropertyValue(this._caminhoConstrutor); set => this.SetProperty(this._caminhoConstrutor, this._caminhoConstrutor = value); }

    public string NomeControle { get => this.GetPropertyValue(this._nomeControle); set => this.SetProperty(this._nomeControle, this._nomeControle = value); }
}