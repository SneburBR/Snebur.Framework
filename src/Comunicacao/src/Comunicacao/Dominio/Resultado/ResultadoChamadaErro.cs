namespace Snebur.Comunicacao
{
    public abstract class ResultadoChamadaErro : ResultadoChamada
    {

        #region Campos Privados

        private string _mensagemErro;
        private object _erro;

        #endregion

        public string MensagemErro { get => this.RetornarValorPropriedade(this._mensagemErro); set => this.NotificarValorPropriedadeAlterada(this._mensagemErro, this._mensagemErro = value); }

        public object Erro { get => this.RetornarValorPropriedade(this._erro); set => this.NotificarValorPropriedadeAlterada(this._erro, this._erro = value); }
    }
}