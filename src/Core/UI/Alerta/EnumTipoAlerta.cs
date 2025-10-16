namespace Snebur.UI;

public enum EnumTipoAlerta
{
    [UndefinedEnumValue] Undefined = -1,
    Atencao,
    Informacao,
    Sucesso,
    Erro,
    Pergunta,
    Carregando,
    CarregandoSemLoader
}