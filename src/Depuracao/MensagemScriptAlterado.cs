namespace Snebur.Depuracao;

public class MensagemScriptAlterado : Mensagem
{

    #region Campos Privados

    private string _nomeArquivo;

    public MensagemScriptAlterado(string nomeArquivo)
    {
        this._nomeArquivo = nomeArquivo;
    }

    #endregion

    public string NomeArquivo { get => this.GetPropertyValue(this._nomeArquivo); set => this.SetProperty(this._nomeArquivo, this._nomeArquivo = value); }
}