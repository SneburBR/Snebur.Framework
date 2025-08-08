namespace Snebur.Dominio.Atributos;
public interface IIndexAttribute
{
    bool IsUnique { get; }
    List<FiltroPropriedadeIndexar>? Filtros { get; }
}
public interface ICompositeIndexAttribute : IIndexAttribute
{
    List<PropriedadeIndexar> Propriedades { get; }
}