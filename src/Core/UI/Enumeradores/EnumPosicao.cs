namespace Snebur.UI;

public enum EnumPosicao
{
    [UndefinedEnumValue] Undefined = -1,
    [Rotulo("Vazio")]
    Vazio = BaseEnumApresentacao.Vazio,
    Nenhum = 0,
    Esquerda = 1,
    Direita = 2,
    Superior = 4,
    Inferior = 8,
    Tudo = 16
}