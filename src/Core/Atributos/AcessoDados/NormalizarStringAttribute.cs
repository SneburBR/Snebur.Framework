using System.Diagnostics.CodeAnalysis;

namespace Snebur.Dominio.Atributos;

public abstract class NormalizarStringAttribute : BaseAtributoDominio, INormalizarString
{
    public abstract string? Normalizar([NotNullIfNotNull(nameof(valor))] string? valor);
}
