namespace Snebur.UI;

public enum EnumTipografia
{
    [UndefinedEnumValue] Undefined = -1,
    [Rotulo("Vazio")]
    Vazio = BaseEnumApresentacao.Vazio,
    h1,
    h2,
    h3,
    h4,
    h5,
    h6,
    h7,
    Titulo,
    SubTitulo,
    SubTitulo2,
    Normal,
    Corpo,
    Corpo2,
    Descricao,
    Descricao2,
    BotaoCaixaAlta,
    LinhaCaixaAlta
}