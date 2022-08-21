
using System;

namespace Snebur.Depuracao
{
    public class MensagemPing : Mensagem
    {

		#region Campos Privados

        private bool _ping;
        private DateTime _dataHora;

		#endregion

        public bool Ping { get => this.RetornarValorPropriedade(this._ping); set => this.NotificarValorPropriedadeAlterada(this._ping, this._ping = value); }

        public DateTime DataHora { get => this.RetornarValorPropriedade(this._dataHora); set => this.NotificarValorPropriedadeAlterada(this._dataHora, this._dataHora = value); }
    }

}