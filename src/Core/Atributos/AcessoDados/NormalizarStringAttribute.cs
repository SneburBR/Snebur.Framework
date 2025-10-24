using System.Diagnostics.CodeAnalysis;

namespace Snebur.Dominio.Atributos;

public abstract class NormalizarStringAttribute : BaseAtributoDominio, INormalizarString
{
    public abstract string? Normalizar([NotNullIfNotNull(nameof(valor))] string? valor);
    public abstract string[] GetSqlFunctions();

    public string BuildComposedFunctionCall(string columnName)
    {
        var sqlFunctions = this.GetSqlFunctions().ToList();
        var expression = $"[{columnName}]";

        var composedFunctionCall = sqlFunctions
            .Aggregate(expression, (inner, outer) => $"{outer}({inner})");
       
        return composedFunctionCall;
    }
}
