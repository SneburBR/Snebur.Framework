using System.ComponentModel;

namespace Snebur.Servicos;

public enum EnumTipoLogDesempenho
{
    [UndefinedEnumValue]
    Undefined = -1,
    [Description("Lentidão no serviço comunicação")]
    LentidaoServicoComunicacao = 1,
    [Description("Lentidão no renderizador")]
    Renderizador = 2
}