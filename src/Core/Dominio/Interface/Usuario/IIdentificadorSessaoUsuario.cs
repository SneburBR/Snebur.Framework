namespace Snebur.Dominio;

public interface IIdentificadorSessaoUsuario
{
    [ValidacaoRequerido]
    [ValidacaoUnico]
    Guid IdentificadorSessaoUsuario { get; set; }
}
