using Snebur.Dominio;
using System;

namespace Snebur.Comunicacao
{
    public class ResultadoSessaoUsuarioInvalida : ResultadoChamada
    {

        #region Campos Privados


        #endregion

        public EnumEstadoSessaoUsuario EstadoSessaoUsuario { get; }

        public Guid IdentificadorSessaoUsuario { get; }

        public ResultadoSessaoUsuarioInvalida(EnumEstadoSessaoUsuario estadoSessaoUsuario,
                                              Guid identificadorSessaoUsuario)
        {
            this.EstadoSessaoUsuario = estadoSessaoUsuario;
            this.IdentificadorSessaoUsuario = identificadorSessaoUsuario;
        }
    }
}