namespace Snebur.Dominio;

[Flags]
public enum EnumOpcoesAlterarPropriedade
{
    [UndefinedEnumValue] Undefined = -1,
    Nenhuma = 0,
    NotificarNovoCadastro = 2,
    VerificarAlteracaoNoBanco = 4,
    AtualizarDataHoraFimAlteracao = 8,
    IgnorarZeroIgualNull = 16,
    IgnorarValorAntigoNull = 32,
    Proteger = 64,
}