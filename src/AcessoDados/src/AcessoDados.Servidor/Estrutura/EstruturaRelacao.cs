namespace Snebur.AcessoDados.Estrutura
{
    internal abstract class EstruturaRelacao : EstruturaPropriedade
    {
        internal bool IsColecao { get; }

        internal EnumTipoRelacao TipoRelacao { get; }

        internal EstruturaRelacao(PropertyInfo propriedade, EstruturaEntidade estruturaEntidade) : base(propriedade, estruturaEntidade)
        {
            this.TipoRelacao = this.RetornarTipoRelacao();
            this.IsColecao = this.RetornarIsColecao();
        }

        private bool RetornarIsColecao()
        {
            switch (this.TipoRelacao)
            {
                case EnumTipoRelacao.RelacaoFilhos:
                case EnumTipoRelacao.RelacaoNn:
                case EnumTipoRelacao.RelacaoNnEspecializada:

                    return true;

                default:
                    return false;
            }
        }

        private EnumTipoRelacao RetornarTipoRelacao()
        {
            switch (this)
            {
                case EstruturaRelacaoPai estruturaPai:

                    return EnumTipoRelacao.RelacaoPai;

                case EstruturaRelacaoUmUm estruturaRelacaoUmUm:

                    return EnumTipoRelacao.RelacaoUmUm;

                case EstruturaRelacaoUmUmReversa estruturaRelacaoUmUm:

                    return EnumTipoRelacao.RelacaoUmUmReversa;

                case EstruturaRelacaoFilhos estruturaRelacaoFilhos:

                    return EnumTipoRelacao.RelacaoFilhos;

                case EstruturaRelacaoNn estruturaRelacaoNn:

                    return EnumTipoRelacao.RelacaoNn; ;

                default:

                    throw new ErroNaoSuportado($"A tipo da estrutura relação {this.GetType().Name} não é suportado");
            }
        }
    }
}