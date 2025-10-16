namespace Snebur.Dominio;

public enum EnumDiaSemana
{
    [UndefinedEnumValue] Undefined = -1,

    [Rotulo("Domingo")]
    Domingo = 0,
    [Rotulo("Segunda-feira")]
    SegundaFeira = 1,
    [Rotulo("Terça-feira")]
    TercaFeira = 2,
    [Rotulo("Quarta-feira")]
    QuartaFeira = 3,
    [Rotulo("Quinta-feira")]
    QuintaFeira = 4,
    [Rotulo("Sexta-feira")]
    SextaFeira = 5,
    [Rotulo("Sábado")]
    Sabado = 6,
}