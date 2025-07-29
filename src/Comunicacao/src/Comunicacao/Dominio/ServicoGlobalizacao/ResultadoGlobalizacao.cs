using Snebur.Dominio;
using System.Collections.Generic;

namespace Snebur.Comunicacao
{

    public class ResultadoGlobalizacao : BaseViewModel
    {
        #region Campos Privados

        private string? _jsonIdiomaBase64;
        private string? _jsonCulturaBase64;

        #endregion

        public string? JsonIdiomaBase64 { get => this.RetornarValorPropriedade(this._jsonIdiomaBase64); set => this.NotificarValorPropriedadeAlterada(this._jsonIdiomaBase64, this._jsonIdiomaBase64 = value); }

        public string? JsonCulturaBase64 { get => this.RetornarValorPropriedade(this._jsonCulturaBase64); set => this.NotificarValorPropriedadeAlterada(this._jsonCulturaBase64, this._jsonCulturaBase64 = value); }

        public List<DominioGlobalizacao> Dominios { get; set; } = new();

        public List<TelaGlobalizacao> Telas { get; set; } = new();
    }

}