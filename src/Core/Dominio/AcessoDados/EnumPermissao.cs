namespace Snebur.AcessoDados.Seguranca;

public enum EnumPermissao
{
    [UndefinedEnumValue] Undefined = -1,
    Autorizado = 1,
    AvalistaRequerido = 2,
    Negado = -99
}