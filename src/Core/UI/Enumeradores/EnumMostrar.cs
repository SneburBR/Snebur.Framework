namespace Snebur.UI;

public enum EnumMostrar
{
    [UndefinedEnumValue] Undefined = -1,
    [Rotulo("Vazio")]
    Vazio = BaseEnumApresentacao.Vazio,
    Normal,
    Pequeno,
    Medido,
    Grande,
    SubTitulo,
    Titulo
}