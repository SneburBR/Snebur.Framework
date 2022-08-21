using System;

namespace Snebur.Dominio
{
    public class InformacaoSessaoUsuario : InformacaoSessao, IIdentificadorSessaoUsuario
    {
        public Guid IdentificadorSessaoUsuario { get; set; }
    }
}
