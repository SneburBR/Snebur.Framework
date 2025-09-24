namespace Snebur.AcessoDados;

public class EntidadeSalvaInfo : BaseAcessoDados
{
    public required long Id { get; set; }

    public required Guid IdentificadorUnicoEntidade { get; set; }

    public required string CaminhoTipoEntidadeSalva { get; set; }

    public List<PropriedadeComputada> PropriedadesComputada { get; set; } = new List<PropriedadeComputada>();
}
