namespace Snebur.AcessoDados;

public class PropriedadeComputada : BaseAcessoDados
{
    public required string NomePropriedade { get; set; }

    public required object? Valor { get; set; }
}