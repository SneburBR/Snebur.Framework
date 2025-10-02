using Snebur.Reflexao;

namespace Snebur.Comunicacao;

public class ParametroChamadaTipoPrimario : ParametroChamada
{
    #region Campos Privados

    private object? _valor;
    private EnumTipoPrimario _tipoPrimarioEnum;

    #endregion

    public object? Valor { get => this.GetPropertyValue(this._valor); set => this.SetProperty(this._valor, this._valor = value); }

    public EnumTipoPrimario TipoPrimarioEnum { get => this.GetPropertyValue(this._tipoPrimarioEnum); set => this.SetProperty(this._tipoPrimarioEnum, this._tipoPrimarioEnum = value); }
}