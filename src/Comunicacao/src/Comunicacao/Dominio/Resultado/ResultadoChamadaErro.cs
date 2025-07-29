using Snebur.Dominio.Atributos;

namespace Snebur.Comunicacao.Dominio.Resultado
{
    public abstract class ResultadoChamadaErro : ResultadoChamada
    {

        #region Campos Privados

        private int _statusCode;
        private string? _mensagemErro;
        private object? _erro;

        #endregion

        [CampoProtegido]
        public string? MensagemErro { get => this.RetornarValorPropriedade(this._mensagemErro); set => this.NotificarValorPropriedadeAlterada(this._mensagemErro, this._mensagemErro = value); }
        public object? Erro { get => this.RetornarValorPropriedade(this._erro); set => this.NotificarValorPropriedadeAlterada(this._erro, this._erro = value); }
        public int StatusCode { get => this.RetornarValorPropriedade(this._statusCode); set => this.NotificarValorPropriedadeAlterada(this._statusCode, this._statusCode = value); }
    }
}