using Snebur.Reflexao;

namespace Snebur.Comunicacao;

public class ResultadoChamadaListaTipoPrimario : ResultadoChamadaLista
{
    #region Campos Privados

    private EnumTipoPrimario _tipoPrimarioEnum;

    #endregion

    public List<object> Valores { get; set; } = new();

    public EnumTipoPrimario TipoPrimarioEnum { get => this.GetPropertyValue(this._tipoPrimarioEnum); set => this.SetProperty(this._tipoPrimarioEnum, this._tipoPrimarioEnum = value); }
}