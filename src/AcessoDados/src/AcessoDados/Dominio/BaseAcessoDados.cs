namespace Snebur.AcessoDados;

public abstract class BaseAcessoDados : BaseDominio
{
    #region Campos Privados

    private string? _mensagemErro;
    private bool _falhaAutenticacao;

    #endregion

    //public string Erro { get; set; }



    public string? MensagemErro { get => this.GetPropertyValue(this._mensagemErro); set => this.SetProperty(this._mensagemErro, this._mensagemErro = value); }

    public bool FalhaAutenticacao { get => this.GetPropertyValue(this._falhaAutenticacao); set => this.SetProperty(this._falhaAutenticacao, this._falhaAutenticacao = value); }

    public List<string> Comandos { get; set; } = new List<string>();

    public BaseAcessoDados()
    {
    }
}