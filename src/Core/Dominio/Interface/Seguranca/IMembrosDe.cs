using Snebur.Dominio.Atributos;

namespace Snebur.Dominio;

[IgnorarInterfaceTS]
public interface IMembrosDe : IEntidadeSeguranca
{
    IEnumerable<IGrupoUsuario> MembrosDe { get; }
}
