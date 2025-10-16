namespace Snebur.UI;

public enum EnumResultadoAlerta
{
    [UndefinedEnumValue] Undefined = -1,
    Sim,
    Nao,
    Continuar,
    Cancelar,
    Fechar,
    [Rotulo("OK")]
    Ok,
    EfetuarPgto
}