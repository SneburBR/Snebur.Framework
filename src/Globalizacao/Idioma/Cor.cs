using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Snebur.Dominio.Atributos;

namespace Snebur.Globalizacao
{
    public class Cor : BaseGlobalizacao
    {
		#region Campos Privados

    	private string _nome;
    	private string _identificador;
    	private string _hexa;
    	private string _rgb;

		#endregion

    	public string Nome { get => this.RetornarValorPropriedade(this._nome); set => this.NotificarValorPropriedadeAlterada(this._nome, this._nome = value); }
    
    	public string Identificador { get => this.RetornarValorPropriedade(this._identificador); set => this.NotificarValorPropriedadeAlterada(this._identificador, this._identificador = value); }
    
    	public string Hexa { get => this.RetornarValorPropriedade(this._hexa); set => this.NotificarValorPropriedadeAlterada(this._hexa, this._hexa = value); }
    
    	public string Rgb { get => this.RetornarValorPropriedade(this._rgb); set => this.NotificarValorPropriedadeAlterada(this._rgb, this._rgb = value); }

        #region Construtores 

        public Cor()
        {
        }
        [IgnorarConstrutorTS()]
        public Cor(string nome, string identificador, string hexa, string rgb)
        {
            this.Nome = nome;
            this.Hexa = hexa;
            this.Identificador = identificador;
            this.Rgb = rgb;
        }
        #endregion
    }
}