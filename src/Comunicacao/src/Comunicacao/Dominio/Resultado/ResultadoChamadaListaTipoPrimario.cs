using Snebur.Reflexao;
using System.Collections.Generic;

namespace Snebur.Comunicacao
{
    public class ResultadoChamadaListaTipoPrimario : ResultadoChamadaLista
    {
        #region Campos Privados

        private EnumTipoPrimario _tipoPrimarioEnum;

        #endregion

        public List<object> Valores { get; set; }

        public EnumTipoPrimario TipoPrimarioEnum { get => this.RetornarValorPropriedade(this._tipoPrimarioEnum); set => this.NotificarValorPropriedadeAlterada(this._tipoPrimarioEnum, this._tipoPrimarioEnum = value); }
    }
}