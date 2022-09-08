namespace Snebur.AcessoDados.Seguranca
{
    internal class AutorizacaoPermissaoEntidade
    {
        internal EstruturaPermissaoEntidade EstruturaPermissaoEntidade { get; }

        internal EnumPermissao Permissao { get; }

        internal AutorizacaoPermissaoEntidade(EstruturaPermissaoEntidade estruturaPermissaoEntidade, EnumPermissao permissao)
        {
            this.EstruturaPermissaoEntidade = estruturaPermissaoEntidade;
            this.Permissao = permissao;
        }
    }
}
