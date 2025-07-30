using Snebur.Dominio.Atributos;

namespace Snebur.Comunicacao.Dominio.ServicoAplicacao.ConfiguracaoAplicacao
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
        public string? UrlVisualizarImagem { get => this.GetPropertyValue(this._urlVisualizarImagem); set => this.SetProperty(this._urlVisualizarImagem, this._urlVisualizarImagem = value); }
    }
}