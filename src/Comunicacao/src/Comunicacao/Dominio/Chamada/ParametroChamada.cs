using Snebur.Dominio.Atributos;

namespace Snebur.Comunicacao.Dominio.Chamada
{
    [Plural("ParametrosChamada")]
    public abstract class ParametroChamada : BaseComunicao
    {
        #region Campos Privados

        private string? _nome;
        private string? _nomeTipoParametro;
        private string? _assemblyQualifiedName;

        #endregion

        public string? Nome { get => this.GetPropertyValue(this._nome); set => this.SetProperty(this._nome, this._nome = value); }

        public string? NomeTipoParametro { get => this.GetPropertyValue(this._nomeTipoParametro); set => this.SetProperty(this._nomeTipoParametro, this._nomeTipoParametro = value); }

        public string? AssemblyQualifiedName { get => this.GetPropertyValue(this._assemblyQualifiedName); set => this.SetProperty(this._assemblyQualifiedName, this._assemblyQualifiedName = value); }
    }
}