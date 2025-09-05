using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Mapeamento
{
    internal abstract class EstruturaEntidadeApelidoRelacao : EstruturaEntidadeApelido
    {
        internal EstruturaRelacao EstruturaRelacao { get; }

        internal EstruturaEntidadeApelidoRelacao(
            BaseMapeamentoConsulta mapeamentoConsulta,
            string apelidoEntidadeMapeada,
            EstruturaEntidade estruturaEntidade,
            EstruturaRelacao? estruturaRelacao
            ) : base(mapeamentoConsulta, apelidoEntidadeMapeada, estruturaEntidade)
        {
            this.EstruturaRelacao = estruturaRelacao;
        }
    }
}
