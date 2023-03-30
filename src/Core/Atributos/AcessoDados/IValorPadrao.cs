namespace Snebur.Dominio.Atributos
{
    [IgnorarInterfaceTS]
    public interface IConverterValorPadrao
    {
        object RetornarValorPadrao(object contexto,
                                   Entidade entidadeCorrente,
                                   object valorPropriedade);
    }


    [IgnorarInterfaceTS]
    public interface IValorPadrao : IConverterValorPadrao
    {
        bool IsTipoNullableRequerido { get; }
        bool IsValorPadraoOnUpdate { get; }
    }
}