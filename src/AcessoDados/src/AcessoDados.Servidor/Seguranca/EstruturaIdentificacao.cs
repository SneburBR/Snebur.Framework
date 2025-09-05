namespace Snebur.AcessoDados.Seguranca
{
    internal class EstruturaIdentificacao
    {
        internal IIdentificacao Identificacao { get; set; }

        internal Dictionary<string, EstruturaPermissaoEntidade> PermissoesEntidade { get; set; } = new Dictionary<string, EstruturaPermissaoEntidade>();

        internal EstruturaIdentificacao(IIdentificacao identificacao, List<IPermissaoEntidade> permissoes)
        {
            this.Identificacao = identificacao;

            foreach (var permissao in permissoes)
            {
                this.PermissoesEntidade.Add(permissao.NomeTipoEntidadePermissao, new EstruturaPermissaoEntidade(permissao));
            }
        }
    }
}