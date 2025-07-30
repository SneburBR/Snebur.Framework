using Snebur.Seguranca;

namespace Snebur.Comunicacao.Dominio
{
    public class Cabecalho : BaseComunicao /*, IIdentificadorProprietario, IIdentificadorAplicacao */
    {
        #region Campos Privados

        private string? _identificadorProprietario;
        private string? _urlOrigem;

        #endregion

        public string? IdentificadorProprietario { get => this.GetPropertyValue(this._identificadorProprietario); set => this.SetProperty(this._identificadorProprietario, this._identificadorProprietario = value); }
        public CredencialServico CredencialServico { get; set; } = new();

        public CredencialUsuario CredencialUsuario { get;   set; } = new();

        public CredencialUsuario CredencialAvalista { get;   set; } = new();

        public string? UrlOrigem { get => this.GetPropertyValue(this._urlOrigem); set => this.SetProperty(this._urlOrigem, this._urlOrigem = value); }
    }
}