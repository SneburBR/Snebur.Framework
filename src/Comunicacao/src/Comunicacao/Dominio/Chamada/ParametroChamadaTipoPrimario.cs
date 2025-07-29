using Snebur.Reflexao;

namespace Snebur.Comunicacao.Dominio.Chamada
{
    public class ParametroChamadaTipoPrimario : ParametroChamada
    {
        #region Campos Privados

        private object? _valor;
        private EnumTipoPrimario _tipoPrimarioEnum;

        #endregion

        public object? Valor { get => this.RetornarValorPropriedade(this._valor); set => this.NotificarValorPropriedadeAlterada(this._valor, this._valor = value); }

        public EnumTipoPrimario TipoPrimarioEnum { get => this.RetornarValorPropriedade(this._tipoPrimarioEnum); set => this.NotificarValorPropriedadeAlterada(this._tipoPrimarioEnum, this._tipoPrimarioEnum = value); }
    }
}