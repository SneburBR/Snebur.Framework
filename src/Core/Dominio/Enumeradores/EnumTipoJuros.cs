namespace Snebur.Dominio;

public enum EnumTipoJuros
{
    [UndefinedEnumValue] Undefined = -1,
    SemJuros = 0,
    Simples = 1,
    Composto = 2,
    Amortizado = 3
}