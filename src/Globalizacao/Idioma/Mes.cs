﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Snebur.Dominio.Atributos;

namespace Snebur.Globalizacao
{
    public class Mes : BaseGlobalizacao
    {
		#region Campos Privados

    	private int _posicao;
    	private string _abreviatura;
    	private string _nome;

		#endregion

    	public int Posicao { get => this.RetornarValorPropriedade(this._posicao); set => this.NotificarValorPropriedadeAlterada(this._posicao, this._posicao = value); }

    	public string Abreviatura { get => this.RetornarValorPropriedade(this._abreviatura); set => this.NotificarValorPropriedadeAlterada(this._abreviatura, this._abreviatura = value); }

    	public string Nome { get => this.RetornarValorPropriedade(this._nome); set => this.NotificarValorPropriedadeAlterada(this._nome, this._nome = value); }

    	public Mes()
    	{
    	}
        [IgnorarConstrutorTS()]
        public Mes(int posicao, string descricao, string abreviatura)
    	{
    		this.Posicao = posicao;
    		this.Nome = descricao;
    		this.Abreviatura = abreviatura;
    	}
    }
}