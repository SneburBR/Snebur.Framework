namespace Snebur.Dominio.Atributos
{
    [IgnorarInterfaceTS]
    public interface IConverterValorPadrao
    {
        object RetornarValorPadrao(object contexto,
                                   Entidade entidadeCorrente);
    }


    [IgnorarInterfaceTS]
    public interface IValorPadrao : IConverterValorPadrao
    {
        bool TipoNullableRequerido { get; }
    }
}