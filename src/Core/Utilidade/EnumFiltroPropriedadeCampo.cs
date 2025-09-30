namespace Snebur.Utilidade;

public enum EnumFiltroPropriedadeCampo
{
    [UndefinedEnumValue]
    Undefined = -1,
    Todas = 1,
    IgnorarTipoBase = 2,
    IgnorarChavePrimaria = 4,
    IgnorarPropriedadeProtegida = 8,
    IgnorarChaveEstrangeira = 16
}