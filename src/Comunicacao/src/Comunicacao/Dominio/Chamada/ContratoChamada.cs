using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using System;
using System.Collections.Generic;

namespace Snebur.Comunicacao
{
    [Plural("ContratoChamada")]
    public class ContratoChamada : BaseComunicao
    {
        #region Campos Privados

        private Guid _identificadorSessaoUsuario;
        private string _operacao;
        private DateTime _dataHora;
        private bool _async;

        #endregion

        public Cabecalho Cabecalho { get; set; }

        public InformacaoSessao InformacaoSessao { get; set; }
        public Guid IdentificadorSessaoUsuario { get => this.RetornarValorPropriedade(this._identificadorSessaoUsuario); set => this.NotificarValorPropriedadeAlterada(this._identificadorSessaoUsuario, this._identificadorSessaoUsuario = value); }

        public string Operacao { get => this.RetornarValorPropriedade(this._operacao); set => this.NotificarValorPropriedadeAlterada(this._operacao, this._operacao = value); }

        public DateTime DataHora { get => this.RetornarValorPropriedade(this._dataHora); set => this.NotificarValorPropriedadeAlterada(this._dataHora, this._dataHora = value); }

        public bool Async { get => this.RetornarValorPropriedade(this._async); set => this.NotificarValorPropriedadeAlterada(this._async, this._async = value); }

        public List<ParametroChamada> Parametros { get; set; } = new List<ParametroChamada>();

        public ContratoChamada()
        {

        }
    }
}