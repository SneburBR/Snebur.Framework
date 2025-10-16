namespace Snebur.Dominio.Atributos;

[IgnorarGlobalizacao]
public enum EnumOrdenacaoNovoRegistro
{
    [UndefinedEnumValue] Undefined = -1,
    Inicio = 1,
    Fim = 2,
    Aleatorio = 3
}