using Snebur.Dominio.Atributos;

namespace Snebur.Dominio;

public interface IIdentificadorSessaoUsuario
{
    [ValidacaoRequerido]
    [ValidacaoUnico]
    Guid IdentificadorSessaoUsuario { get; set; }
}
