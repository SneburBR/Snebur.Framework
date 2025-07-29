using Snebur.Dominio;

namespace Snebur.Comunicacao.Mensageiro
{
    public class ContratoMensageiro : BaseDominio
    {

        #region Campos Privados

        private string? _nomeRecurso;

        #endregion

        public BaseDominio? Remetente { get; set; } 

        public BaseDominio? Destinatario { get; set; } 

        public string? NomeRecurso { get => this.RetornarValorPropriedade(this._nomeRecurso); set => this.NotificarValorPropriedadeAlterada(this._nomeRecurso, this._nomeRecurso = value); }

        public BaseDominio? ValorParametro { get; set; } 
    }
}