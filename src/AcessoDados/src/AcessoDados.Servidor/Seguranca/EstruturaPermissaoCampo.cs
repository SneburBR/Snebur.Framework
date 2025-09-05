namespace Snebur.AcessoDados.Seguranca
{
    internal class EstruturaPermissaoCampo
    {
        internal IPermissaoCampo PermissaoCampo { get; }

        internal EstruturaPermissaoCampo(IPermissaoCampo permissaoCampo)
        {
            Guard.NotNull(permissaoCampo);
            Guard.NotNull(permissaoCampo.Leitura);
            Guard.NotNull(permissaoCampo.Atualizar);

            this.PermissaoCampo = permissaoCampo;
        }
    }
}
