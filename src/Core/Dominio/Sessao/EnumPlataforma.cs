using System.ComponentModel;

namespace Snebur.Dominio;

[IgnorarGlobalizacao]
public enum EnumPlataforma
{
    [UndefinedEnumValue] Undefined = -1,

    [Description("PC")]
    PC = 1,
    [Description("Celular")]
    Celular = 2,
    [Description("Tablet")]
    Tablet = 3,

    [Description("Desconhecido")]
    Desconhecido = 99,
}