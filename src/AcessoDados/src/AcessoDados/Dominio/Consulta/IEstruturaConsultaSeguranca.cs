namespace Snebur.AcessoDados.Seguranca;

public interface IEstruturaConsultaSeguranca
{
    //[IgnorarPropriedadeTS]
    //[IgnorarPropriedadeTSReflexao]
    List<string> PropriedadesAbertas { get; set; }

    [IgnorarPropriedade]
    [IgnorarPropriedadeTSReflexao]
    List<string>? PropriedadesAutorizadas { get; }

    [IgnorarMetodoTS]
    void AtribuirPropriedadeAutorizadas(List<string> propriedadesAutorizadas);
}
