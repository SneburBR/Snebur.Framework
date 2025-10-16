namespace Snebur.UI;

public enum EnumPesoFonte
{
    [UndefinedEnumValue] Undefined = -1,
    [Rotulo("Vazio")]
    Vazio = BaseEnumApresentacao.Vazio,
    SuperLeve = 1,
    Leve = 2,
    Normal = 3,
    Negrito = 4,
    Pesado = 5,
    SuperPesado = 6
}