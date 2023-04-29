using Snebur.Dominio;
using System;

namespace Snebur.Comunicacao
{
    public class ResultadoSessaoUsuarioInvalida : ResultadoChamada
    {

        #region Campos Privados


        #endregion

        public EnumStatusSessaoUsuario StatusSessaoUsuario { get; }

        public Guid IdentificadorSessaoUsuario { get; }

        public ResultadoSessaoUsuarioInvalida(EnumStatusSessaoUsuario statusSessaoUsuario,
                                              Guid identificadorSessaoUsuario)
        {
            this.StatusSessaoUsuario = statusSessaoUsuario;
            this.IdentificadorSessaoUsuario = identificadorSessaoUsuario;
        }
    }
}