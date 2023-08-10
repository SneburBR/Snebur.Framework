namespace Snebur.AcessoDados
{
    public enum EnumFlagBancoNaoSuportado
    {
        SemRestricao = 0,
        OffsetFetch = 2,
        ColunaNomeTipoEntidade = 4,
        SessaoUsuario = 8,
        Migracao = 16,
        DataHoraUtc = 32,
        SessaoUsuarioHerdada = 64,
    }
}
