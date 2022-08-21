using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Snebur.Dominio.Atributos;
using Snebur.Dominio;
using Snebur.Reflexao;

namespace Snebur.Comunicacao
{

    public class ParametroChamadaListaTipoPrimario : ParametroChamadaLista
    {
		#region Campos Privados

        private EnumTipoPrimario _tipoPrimarioEnum;

		#endregion

        public List<object> Lista { get; set; }

        public EnumTipoPrimario TipoPrimarioEnum { get => this.RetornarValorPropriedade(this._tipoPrimarioEnum); set => this.NotificarValorPropriedadeAlterada(this._tipoPrimarioEnum, this._tipoPrimarioEnum = value); }
    }
}