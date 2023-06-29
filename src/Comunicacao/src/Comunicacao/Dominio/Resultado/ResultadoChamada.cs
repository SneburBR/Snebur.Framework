using System;

namespace Snebur.Comunicacao
{
    public abstract class ResultadoChamada : BaseComunicao
    {

		#region Campos Privados

        private string _nomeServico;
        private DateTime _dataHora;
        private string _operacao;

		#endregion

        public string NomeServico { get => this.RetornarValorPropriedade(this._nomeServico); set => this.NotificarValorPropriedadeAlterada(this._nomeServico, this._nomeServico = value); }
        public DateTime DataHora { get => this.RetornarValorPropriedade(this._dataHora); set => this.NotificarValorPropriedadeAlterada(this._dataHora, this._dataHora = value); }
        public string Operacao { get => this.RetornarValorPropriedade(this._operacao); set => this.NotificarValorPropriedadeAlterada(this._operacao, this._operacao = value); }

    }
}