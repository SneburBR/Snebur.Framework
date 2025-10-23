using System.Diagnostics.CodeAnalysis;

namespace Snebur.Dominio;

public interface INormalizarString
{
    public string? Normalizar([NotNullIfNotNull(nameof(valor))] string? valor);
}

