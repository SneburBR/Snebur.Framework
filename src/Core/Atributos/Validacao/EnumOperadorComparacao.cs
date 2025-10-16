namespace Snebur.Dominio.Atributos;

[IgnorarTSReflexao]
public enum EnumOperadorComparacao
{
    [UndefinedEnumValue] Undefined = -1,
    Nenhum = 0,
    Igual = 1,
    Diferente = 2,
    MaiorQue = 3,
    MenorQue = 4,
    MaiorIgualA = 5,
    MenorIgualA = 6
}