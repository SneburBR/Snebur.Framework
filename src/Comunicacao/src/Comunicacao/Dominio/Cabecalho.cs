using Snebur.Seguranca;

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