﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;

namespace Snebur.AcessoDados
{
    public class ResultadoConsulta : Resultado
    {
		#region Campos Privados

        private int _totalRegistros;

		#endregion

        public ListaEntidades<IEntidade> Entidades { get; set; } = new ListaEntidades<IEntidade>();

        public int TotalRegistros { get => this.RetornarValorPropriedade(this._totalRegistros); set => this.NotificarValorPropriedadeAlterada(this._totalRegistros, this._totalRegistros = value); }

        //public PaginacaoConsulta PaginacaoConsulta { get; set; }
    }
}