using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Mapeamento
{
    internal class FiltroMapeamentoEntre : BaseFiltroMapeamento
    {
        internal long MenorId { get; set; }

        internal long MaiorId { get; set; }

        internal string NomeTipoEntidade { get; set; }

        internal FiltroMapeamentoEntre(BaseFiltroMapeamento filtroMapeamentoBase,
                                       long menorId,
                                       long maiorId,
                                       string nomeTipoEntidade) : base(filtroMapeamentoBase)
        {
            this.MenorId = menorId;
            this.MaiorId = maiorId;
            this.NomeTipoEntidade = nomeTipoEntidade;
        }

        internal FiltroMapeamentoEntre(long menorId,
                                       long maiorId,
                                       string nomeTipoEntidade) :
                                       this(new FiltroMapeamentoVazio(),
                                            menorId,
                                            maiorId,
                                            nomeTipoEntidade)
        {
        }

        internal FiltroMapeamentoEntre(long menorId,
                                       long maiorId) :
                                       this(new FiltroMapeamentoVazio(),
                                            menorId,
                                            maiorId,
                                            null)
        {
        }

        internal FiltroMapeamentoEntre(EstruturaCampo estruturaCampoFiltro,
                                      long menorId,
                                      long maiorId) :
                                      this(new FiltroMapeamentoVazio(),
                                           menorId,
                                           maiorId,
                                           null)
        {
            this.EstruturaCampoFiltro = estruturaCampoFiltro;
        }
    }
}