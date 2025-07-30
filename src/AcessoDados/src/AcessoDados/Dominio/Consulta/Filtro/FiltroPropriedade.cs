using Snebur.Reflexao;

namespace Snebur.AcessoDados;


public class FiltroPropriedade : BaseFiltro
{
    #region Campos Privados

    private string? _caminhoPropriedade;
    private EnumTipoPrimario _tipoPrimarioEnum;
    private EnumOperadorFiltro _operador;
    private object? _valor;

    #endregion

    public string? CaminhoPropriedade { get => this.GetPropertyValue(this._caminhoPropriedade); set => this.SetProperty(this._caminhoPropriedade, this._caminhoPropriedade = value); }

    public EnumTipoPrimario TipoPrimarioEnum { get => this.GetPropertyValue(this._tipoPrimarioEnum); set => this.SetProperty(this._tipoPrimarioEnum, this._tipoPrimarioEnum = value); }

    public EnumOperadorFiltro Operador { get => this.GetPropertyValue(this._operador); set => this.SetProperty(this._operador, this._operador = value); }

    public object? Valor { get => this.GetPropertyValue(this._valor); set => this.SetProperty(this._valor, this._valor = value); }

    public FiltroPropriedade()
    {

    }
    [IgnorarConstrutorTS]
    public FiltroPropriedade(PropertyInfo propriedade, EnumOperadorFiltro operador, object valorPropriedade)
    {
        this.CaminhoPropriedade = propriedade.Name;
        this.Operador = operador;
        this.Valor = valorPropriedade;
        this.TipoPrimarioEnum = ReflexaoUtil.RetornarTipoPrimarioEnum(propriedade.PropertyType);
    }
}