namespace Snebur.Dominio;

public enum EnunFlagAlteracaoPropriedade
{
    [UndefinedEnumValue]
    None = 0,
    NotificarNovoCadastro = 2,
    VerificarAlteracaoNoBanco = 4,
    AtualizarDataHoraFimAlteracao = 8
}