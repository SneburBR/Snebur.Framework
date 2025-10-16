namespace Snebur.Dominio;

public enum EnunFlagAlteracaoPropriedade
{
    [UndefinedEnumValue] Undefined = -1,
    None = 0,
    NotificarNovoCadastro = 2,
    VerificarAlteracaoNoBanco = 4,
    AtualizarDataHoraFimAlteracao = 8
}