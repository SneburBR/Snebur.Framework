namespace Snebur.Imagens;

public enum EnumTamanhoGradiente
{
    [Rotulo("closest-side")]
    [UndefinedEnumValue]
    ClosestSide,
    [Rotulo("farthest-side")]
    FarthestSide,
    [Rotulo("closest-corner")]
    ClosestCorner,
    [Rotulo("farthest-corner")]
    FarthestCorner,
}