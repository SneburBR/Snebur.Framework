using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Mapeamento;

internal abstract class BaseFiltroMapeamento
{
    internal bool IsIdTipoEntidade { get; set; } = false;

    internal protected EstruturaCampo? EstruturaCampoFiltro { get; protected set; }

    internal protected BaseFiltroMapeamento? FiltroMapeamentoBase { get; protected set; }

    internal BaseFiltroMapeamento(BaseFiltroMapeamento? filtroMapaementoBase)
    {
        this.FiltroMapeamentoBase = filtroMapaementoBase;
    }

    public override string ToString()
    {
        return $"Type: {this.GetType().Name}, IsIdTipoEntidade: {this.IsIdTipoEntidade}, EstruturaCampoFiltro: {this.EstruturaCampoFiltro}, FiltroMapeamentoBase: {this.FiltroMapeamentoBase}";
    }
}