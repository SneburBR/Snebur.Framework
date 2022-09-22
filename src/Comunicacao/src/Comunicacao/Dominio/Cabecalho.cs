using Snebur.Seguranca;
using System;

namespace Snebur.Comunicacao
{
    public class Cabecalho : BaseComunicao /*, IIdentificadorProprietario, IIdentificadorAplicacao */
    {
        #region Campos Privados

        private string _identificadorProprietario;
        private string _urlOrigem;

        #endregion

        public string IdentificadorProprietario { get => this.RetornarValorPropriedade(this._identificadorProprietario); set => this.NotificarValorPropriedadeAlterada(this._identificadorProprietario, this._identificadorProprietario = value); }
        public CredencialServico CredencialServico { get;   set; }

        public CredencialUsuario CredencialUsuario { get;   set; }

        public CredencialUsuario CredencialAvalista { get;   set; }

        public string UrlOrigem { get => this.RetornarValorPropriedade(this._urlOrigem); set => this.NotificarValorPropriedadeAlterada(this._urlOrigem, this._urlOrigem = value); }


    }
}