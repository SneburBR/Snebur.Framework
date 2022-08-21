using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Snebur.Dominio.Atributos;

namespace Snebur.AcessoDados
{
    public class ConsultaFiltroColecao : BaseAcessoDados
    {
		#region Campos Privados

        private string _relacao;

		#endregion

        public string Relacao { get => this.RetornarValorPropriedade(this._relacao); set => this.NotificarValorPropriedadeAlterada(this._relacao, this._relacao = value); }

        public EstruturaConsulta EstruturaConsulta { get; set; }
    }
}