namespace Snebur.Comunicacao;

public class ResultadoSessaoUsuarioInvalida : ResultadoChamadaErro
{

    #region Campos Privados

    private EnumStatusSessaoUsuario _statusSessaoUsuario;
    private Guid? _identificadorSessaoUsuario;

    #endregion

    public EnumStatusSessaoUsuario StatusSessaoUsuario { get => this.GetPropertyValue(this._statusSessaoUsuario); set => this.SetProperty(this._statusSessaoUsuario, this._statusSessaoUsuario = value); }

    public Guid? IdentificadorSessaoUsuario { get => this.GetPropertyValue(this._identificadorSessaoUsuario); set => this.SetProperty(this._identificadorSessaoUsuario, this._identificadorSessaoUsuario = value); }

    public ResultadoSessaoUsuarioInvalida(EnumStatusSessaoUsuario statusSessaoUsuario,
                                          Guid? identificadorSessaoUsuario,
                                          string mensagemErro)
    {
        this.StatusSessaoUsuario = statusSessaoUsuario;
        this.IdentificadorSessaoUsuario = identificadorSessaoUsuario;
        this.MensagemErro = mensagemErro;
    }
}