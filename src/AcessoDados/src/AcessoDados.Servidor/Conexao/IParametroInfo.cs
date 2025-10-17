using System.Data;

namespace Snebur.AcessoDados;

public interface IParametroInfo
{
    string ParameterName { get; }
    int? Size { get; }
    object? Value { get; }
    SqlDbType SqlDbType { get; }
}
