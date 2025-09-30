namespace Snebur.Dominio;

public enum EnumFiltroImagem
{
    [Rotulo("Exposição")]
    [UndefinedEnumValue]
    Exposicao,
    [Rotulo("Brilho")]
    Brilho,
    [Rotulo("Contraste")]
    Contraste,
    [Rotulo("Saturação")]
    Saturacao,
    [Rotulo("Preto e branco")]
    PretoBranco,
    [Rotulo("Sépia")]
    Sepia,
    [Rotulo("Matriz")]
    Matriz,
    [Rotulo("Inverter")]
    Inverter,
    [Rotulo("Desfoque")]
    Desfoque,
    [Rotulo("Ciano")]
    Ciano,
    [Rotulo("Magenta")]
    Magenta,
    [Rotulo("Amarelo")]
    Amarelo,
}