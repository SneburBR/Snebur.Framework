namespace Snebur.Comunicacao;

public class ResultadoRecuperarSenha : BaseResultadoRecuperarSenha
{

    #region Campos Privados

    private bool _isUsuarioEncontrado;
    private EnumStatusCodigoRecuperarSenha _status;

    #endregion

    public bool IsUsuarioEncontrado { get => this.GetPropertyValue(this._isUsuarioEncontrado); set => this.SetProperty(this._isUsuarioEncontrado, this._isUsuarioEncontrado = value); }

    public EnumStatusCodigoRecuperarSenha Status { get => this.GetPropertyValue(this._status); set => this.SetProperty(this._status, this._status = value); }
}