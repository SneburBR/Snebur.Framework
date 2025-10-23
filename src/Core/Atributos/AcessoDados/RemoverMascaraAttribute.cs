using System.Diagnostics.CodeAnalysis;

namespace Snebur.Dominio.Atributos;

public class RemoverMascaraAttribute : NormalizarStringAttribute
{
    public override string? Normalizar([NotNullIfNotNull(nameof(valor))] string? valor)
    {
        return TextoUtil.RetornarSomenteNumeros(valor);
    }
}
