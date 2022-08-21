using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Snebur.Dominio.Atributos;
using Snebur.Seguranca;
using Snebur.Dominio;

namespace Snebur.Comunicacao
{
    public class Cabecalho : BaseComunicao /*, IIdentificadorProprietario, IIdentificadorAplicacao */
    {
		#region Campos Privados

        private string _urlOrigem;

		#endregion

    	public CredencialServico CredencialServico { get; set; }
    
    	public CredencialUsuario CredencialUsuario { get; set; }

        public CredencialUsuario CredencialAvalista { get; set; }

        public string UrlOrigem { get => this.RetornarValorPropriedade(this._urlOrigem); set => this.NotificarValorPropriedadeAlterada(this._urlOrigem, this._urlOrigem = value); }
 
    }
}