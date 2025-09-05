
namespace Snebur.Depuracao;

public class MensagemEstiloCssAlterado : Mensagem
{

    #region Campos Privados

    private string _nomeArquivo;

    #endregion

    public MensagemEstiloCssAlterado(string nomeArquivo)
    {
        this._nomeArquivo = nomeArquivo;
    }

    public string NomeArquivo { get => this.GetPropertyValue(this._nomeArquivo); set => this.SetProperty(this._nomeArquivo, this._nomeArquivo = value); }
}