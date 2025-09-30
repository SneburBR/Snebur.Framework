namespace Snebur.Dominio.Atributos;

[IgnorarInterfaceTS]
public interface IIndexAttribute
{
    bool IsUnique { get; }
    List<FiltroPropriedadeIndexar>? Filtros { get; }
}

[IgnorarInterfaceTS]
public interface ICompositeIndexAttribute : IIndexAttribute
{
    List<PropriedadeIndexar> Propriedades { get; }
}