using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Mapeamento
{
    internal abstract class BaseFiltroMapeamento
    {
        internal bool IsIdTipoEntidade { get; set; } = false;

        public EstruturaCampo EstruturaCampoFiltro { get; set; }

        internal BaseFiltroMapeamento FiltroMapeamentoBase { get; set; }

        internal BaseFiltroMapeamento(BaseFiltroMapeamento filtroMapaementoBase)
        {
            this.FiltroMapeamentoBase = filtroMapaementoBase;
        }
    }
}