using System.Diagnostics.CodeAnalysis;

namespace Snebur.Dominio.Atributos;

public class RemoverMascaraAttribute : NormalizarStringAttribute
{

    public RemoverMascaraAttribute()
    {
    }
    public override string? Normalizar([NotNullIfNotNull(nameof(valor))] string? valor)
    {
        return TextoUtil.RetornarSomenteNumeros(valor);
    }
  

    public override string[] GetSqlFunctions()
    {
        return [$"[dbo].[fn_OnlyNumbers]"];
    }
}
