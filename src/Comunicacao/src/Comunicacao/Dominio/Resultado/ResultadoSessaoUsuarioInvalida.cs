using Snebur.Dominio;
using System;

namespace Snebur.Comunicacao
{
    public class ResultadoSessaoUsuarioInvalida : ResultadoChamadaErro
    {

		#region Campos Privados

        private EnumStatusSessaoUsuario _statusSessaoUsuario;
        private Guid _identificadorSessaoUsuario;

		#endregion

        public EnumStatusSessaoUsuario StatusSessaoUsuario { get => this.RetornarValorPropriedade(this._statusSessaoUsuario); set => this.NotificarValorPropriedadeAlterada(this._statusSessaoUsuario, this._statusSessaoUsuario = value); }

        public Guid IdentificadorSessaoUsuario { get => this.RetornarValorPropriedade(this._identificadorSessaoUsuario); set => this.NotificarValorPropriedadeAlterada(this._identificadorSessaoUsuario, this._identificadorSessaoUsuario = value); }

        public ResultadoSessaoUsuarioInvalida(EnumStatusSessaoUsuario statusSessaoUsuario,
                                              Guid identificadorSessaoUsuario, 
                                              string mensagemErro)
        {
            this.StatusSessaoUsuario = statusSessaoUsuario;
            this.IdentificadorSessaoUsuario = identificadorSessaoUsuario;
            this.MensagemErro = mensagemErro;
        }
    }
}