using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Servidor.Salvar;

internal class RelacaoChaveEstrageniraDependente
{
    public Entidade EntidadeRelacao { get; }
    public EstruturaRelacaoChaveEstrangeira EstruturaRelacaoChaveEstrangeira { get; }

    public RelacaoChaveEstrageniraDependente(Entidade entidade, EstruturaRelacaoChaveEstrangeira estrutura)
    {
        this.EntidadeRelacao = entidade;
        this.EstruturaRelacaoChaveEstrangeira = estrutura;
    }
}
