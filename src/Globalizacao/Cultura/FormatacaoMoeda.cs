using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Snebur.Globalizacao
{
    public class FormatacaoMoeda : BaseGlobalizacao
    {
		#region Campos Privados

    	private string _descricao;
    	private string _formato;
    	private string _simbolo;

		#endregion

    	public string Descricao { get => this.RetornarValorPropriedade(this._descricao); set => this.NotificarValorPropriedadeAlterada(this._descricao, this._descricao = value); }
    
    	public string Formato { get => this.RetornarValorPropriedade(this._formato); set => this.NotificarValorPropriedadeAlterada(this._formato, this._formato = value); }
    
    	public string Simbolo { get => this.RetornarValorPropriedade(this._simbolo); set => this.NotificarValorPropriedadeAlterada(this._simbolo, this._simbolo = value); }
    }
}