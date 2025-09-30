namespace Snebur.Utilidade;

public enum EnumOpcoesCompararData
{
    [UndefinedEnumValue]
    Undefined = -1,
    /// <summary>
    /// Dia, Mes e Ano
    /// </summary>
    Data = 1,
    //DiasMesAno = Data,
    Dia = 2,
    DiaMes = 3,
    MesAno = 4,
    Ignorar = 5
}