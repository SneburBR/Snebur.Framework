namespace Snebur.Utilidade;

public enum EnumStatusDiretorio
{
    [UndefinedEnumValue]
    Undefined = -1,
    TudoCerto = 1,
    DiretorioNaoExiste = 2,
    SemPermissaoGravacao = 3,
    EspacoInsuficiente = 4
}