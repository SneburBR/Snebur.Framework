using Snebur.Dominio;

namespace Snebur.AcessoDados
{
    public class Ordenacao : BaseAcessoDados
    {

        #region Campos Privados

        private string _caminhoPropriedade;
        private EnumSentidoOrdenacao _sentidoOrdenacaoEnum;


        #endregion

        public string CaminhoPropriedade { get => this.RetornarValorPropriedade(this._caminhoPropriedade); set => this.NotificarValorPropriedadeAlterada(this._caminhoPropriedade, this._caminhoPropriedade = value); }

        public EnumSentidoOrdenacao SentidoOrdenacaoEnum { get => this.RetornarValorPropriedade(this._sentidoOrdenacaoEnum); set => this.NotificarValorPropriedadeAlterada(this._sentidoOrdenacaoEnum, this._sentidoOrdenacaoEnum = value); }
    }
}