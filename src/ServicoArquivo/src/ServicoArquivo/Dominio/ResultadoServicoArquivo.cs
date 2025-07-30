namespace Snebur.ServicoArquivo.Dominio;

public class ResultadoServicoArquivo : BaseDominio
{

    #region Campos Privados

    private long _id;
    private bool _isSucesso;
    private string? _mensagemErro;
    private EnumTipoErroServicoArquivo _tipoErroServicoArquivo;

    #endregion

    public long Id { get => this.GetPropertyValue(this._id); set => this.SetProperty(this._id, this._id = value); }

    public bool IsSucesso { get => this.GetPropertyValue(this._isSucesso); set => this.SetProperty(this._isSucesso, this._isSucesso = value); }

    public string? MensagemErro { get => this.GetPropertyValue(this._mensagemErro); set => this.SetProperty(this._mensagemErro, this._mensagemErro = value); }

    public EnumTipoErroServicoArquivo TipoErroServicoArquivo { get => this.GetPropertyValue(this._tipoErroServicoArquivo); set => this.SetProperty(this._tipoErroServicoArquivo, this._tipoErroServicoArquivo = value); }
}