namespace Snebur.Imagens;

public enum EnumTamanhoGradiente
{
    [UndefinedEnumValue] Undefined = -1,
    [Rotulo("closest-side")]
    ClosestSide,
    [Rotulo("farthest-side")]
    FarthestSide,
    [Rotulo("closest-corner")]
    ClosestCorner,
    [Rotulo("farthest-corner")]
    FarthestCorner,
}