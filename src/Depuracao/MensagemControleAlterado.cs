namespace Snebur.Depuracao
{
    public class MensagemControleAlterado : Mensagem
    {

		#region Campos Privados

        private bool _isScript;
        private string _urlScriptRuntime;
        private string _caminhoConstrutor;
        private string _nomeControle;

		#endregion



        public bool IsScript { get => this.RetornarValorPropriedade(this._isScript); set => this.NotificarValorPropriedadeAlterada(this._isScript, this._isScript = value); }

        public string UrlScriptRuntime { get => this.RetornarValorPropriedade(this._urlScriptRuntime); set => this.NotificarValorPropriedadeAlterada(this._urlScriptRuntime, this._urlScriptRuntime = value); }

        public string CaminhoConstrutor { get => this.RetornarValorPropriedade(this._caminhoConstrutor); set => this.NotificarValorPropriedadeAlterada(this._caminhoConstrutor, this._caminhoConstrutor = value); }

        public string NomeControle { get => this.RetornarValorPropriedade(this._nomeControle); set => this.NotificarValorPropriedadeAlterada(this._nomeControle, this._nomeControle = value); }
    }
}