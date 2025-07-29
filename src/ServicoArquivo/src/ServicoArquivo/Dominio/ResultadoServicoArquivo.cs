using Snebur.Dominio;

namespace Snebur.ServicoArquivo
{
    public class ResultadoServicoArquivo : BaseDominio
    {

        #region Campos Privados

        private long _id;
        private bool _isSucesso;
        private string? _mensagemErro;
        private EnumTipoErroServicoArquivo _tipoErroServicoArquivo;

        #endregion

        public long Id { get => this.RetornarValorPropriedade(this._id); set => this.NotificarValorPropriedadeAlterada(this._id, this._id = value); }

        public bool IsSucesso { get => this.RetornarValorPropriedade(this._isSucesso); set => this.NotificarValorPropriedadeAlterada(this._isSucesso, this._isSucesso = value); }

        public string? MensagemErro { get => this.RetornarValorPropriedade(this._mensagemErro); set => this.NotificarValorPropriedadeAlterada(this._mensagemErro, this._mensagemErro = value); }

        public EnumTipoErroServicoArquivo TipoErroServicoArquivo { get => this.RetornarValorPropriedade(this._tipoErroServicoArquivo); set => this.NotificarValorPropriedadeAlterada(this._tipoErroServicoArquivo, this._tipoErroServicoArquivo = value); }
    }
}