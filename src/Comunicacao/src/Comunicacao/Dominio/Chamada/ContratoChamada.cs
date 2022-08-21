﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Snebur.Dominio.Atributos;
using Snebur.Dominio;

namespace Snebur.Comunicacao
{
    [Plural("ContratoChamada")]
    public class ContratoChamada : BaseComunicao
    {
		#region Campos Privados

    	private string _operacao;
    	private DateTime _dataHora;
        private bool _async;

		#endregion

        public Cabecalho Cabecalho { get; set; }
    
    	public InformacaoSessaoUsuario InformacaoSessaoUsuario { get; set; }
    
    	public string Operacao { get => this.RetornarValorPropriedade(this._operacao); set => this.NotificarValorPropriedadeAlterada(this._operacao, this._operacao = value); }
    
    	public DateTime DataHora { get => this.RetornarValorPropriedade(this._dataHora); set => this.NotificarValorPropriedadeAlterada(this._dataHora, this._dataHora = value); }

        public bool Async { get => this.RetornarValorPropriedade(this._async); set => this.NotificarValorPropriedadeAlterada(this._async, this._async = value); }

        public List<ParametroChamada> Parametros { get; set; } = new List<ParametroChamada>();
    }
}