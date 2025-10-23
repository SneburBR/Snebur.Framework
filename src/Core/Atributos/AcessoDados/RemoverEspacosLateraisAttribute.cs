using Snebur.Dominio.Interface;
using System.Diagnostics.CodeAnalysis;

namespace Snebur.Dominio.Atributos;

public class RemoverEspacosLateraisAttribute : NormalizarStringAttribute
{
    public override string? Normalizar([NotNullIfNotNull(nameof(valor))] string? valor)
    {
        if (valor is null)
        {
            return null;
        }
        return valor.Trim();
    }
}
