namespace Snebur.Dominio
{
    public enum EnumOpcoesAlterarPropriedade
    {
        None = 0,
        NotificarNovoCadastro = 2,
        VerificarAlteracaoNoBanco = 4,
        AtualizarDataHoraFimAlteracao = 8,
        IgnorarZeroIgualNull = 16,
        IgnorarValorAntigoNull = 32
    }
}