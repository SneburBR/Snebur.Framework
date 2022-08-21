using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Snebur.Dominio.Atributos;

namespace Snebur.Globalizacao
{
    public class Pais : BaseGlobalizacao
    {
		#region Campos Privados

    	private string _sigla;
    	private string _sigla3;
    	private string _nome;
    	private string _dDI;
    	private string _idioma;
    	private string _cultura;

		#endregion

    	public string Sigla { get => this.RetornarValorPropriedade(this._sigla); set => this.NotificarValorPropriedadeAlterada(this._sigla, this._sigla = value); }
    
    	public string Sigla3 { get => this.RetornarValorPropriedade(this._sigla3); set => this.NotificarValorPropriedadeAlterada(this._sigla3, this._sigla3 = value); }
    
    	public string Nome { get => this.RetornarValorPropriedade(this._nome); set => this.NotificarValorPropriedadeAlterada(this._nome, this._nome = value); }
    
    	public string DDI { get => this.RetornarValorPropriedade(this._dDI); set => this.NotificarValorPropriedadeAlterada(this._dDI, this._dDI = value); }
    
    	public string Idioma { get => this.RetornarValorPropriedade(this._idioma); set => this.NotificarValorPropriedadeAlterada(this._idioma, this._idioma = value); }
    
    	public string Cultura { get => this.RetornarValorPropriedade(this._cultura); set => this.NotificarValorPropriedadeAlterada(this._cultura, this._cultura = value); }
   
    	public Pais()
    	{
    	}
        [IgnorarConstrutorTS()]
        public Pais(string nome, string sigla, string sigle3, string ddi, string idioma, string cultura)
    	{
    		this.Nome = nome;
    		this.Sigla = sigla;
    		this.Sigla3 = sigle3;
    		this.DDI = ddi;
    		this.Idioma = idioma;
    		this.Cultura = cultura;
    	}
    }
}