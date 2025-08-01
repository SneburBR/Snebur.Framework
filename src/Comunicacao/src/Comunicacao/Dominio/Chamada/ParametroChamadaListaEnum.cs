namespace Snebur.Comunicacao.Dominio.Chamada
{

    public class ParametroChamadaListaEnum : ParametroChamadaLista
    {
        #region Campos Privados

        private string? _nomeTipoEnum;
        private string? _namespaceEnum;

        #endregion

        public string? NomeTipoEnum { get => this.GetPropertyValue(this._nomeTipoEnum); set => this.SetProperty(this._nomeTipoEnum, this._nomeTipoEnum = value); }

        public string? NamespaceEnum { get => this.GetPropertyValue(this._namespaceEnum); set => this.SetProperty(this._namespaceEnum, this._namespaceEnum = value); }

        public List<int> Valores { get; set; } = new();
    }
}