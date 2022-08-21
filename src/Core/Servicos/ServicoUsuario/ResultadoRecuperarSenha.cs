using Snebur.Dominio;

namespace Snebur.Comunicacao
{
    public class ResultadoRecuperarSenha : BaseResultadoRecuperarSenha
    {

        #region Campos Privados

        private bool _isUsuarioEncontrado;
        private EnumEstadoCodigoRecuperarSenha _estado;

        #endregion

        public bool IsUsuarioEncontrado { get => this.RetornarValorPropriedade(this._isUsuarioEncontrado); set => this.NotificarValorPropriedadeAlterada(this._isUsuarioEncontrado, this._isUsuarioEncontrado = value); }

        public EnumEstadoCodigoRecuperarSenha Estado { get => this.RetornarValorPropriedade(this._estado); set => this.NotificarValorPropriedadeAlterada(this._estado, this._estado = value); }

    }
}