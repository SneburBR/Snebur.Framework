namespace Snebur.UI;

public enum EnumBotoesAlerta
{
    [UndefinedEnumValue]
    SimNao,
    Fechar,
    FecharVoltar,
    Nenhum,
    //Cancelar,
    //Continuar,
    [Rotulo("OK")]
    Ok,
    [Rotulo("OK, cancelar")]
    OkCancelar,
    Personalizado
}