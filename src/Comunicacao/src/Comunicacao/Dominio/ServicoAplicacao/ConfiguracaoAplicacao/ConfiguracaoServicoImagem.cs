using Snebur.Dominio.Atributos;

namespace Snebur.Comunicacao
{
    [IgnorarGlobalizacao]
    public class ConfiguracaoServicoImagem : BaseComunicao
    {
        #region Campos Privados

        private string? _urlVisualizarImagem;

        #endregion

        [Rotulo("Url visualiuzar imagem")]
        [ValidacaoRequerido]
        [ValidacaoTextoTamanho(500)]
        public string? UrlVisualizarImagem { get => this.RetornarValorPropriedade(this._urlVisualizarImagem); set => this.NotificarValorPropriedadeAlterada(this._urlVisualizarImagem, this._urlVisualizarImagem = value); }
    }
}