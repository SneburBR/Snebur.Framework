namespace Snebur.Comunicacao.Dominio.Resultado
{
    public abstract class ResultadoChamadaLista : ResultadoChamada
    {
        #region Campos Privados

        private string? _assemblyQualifiedName;

        #endregion

        public string? AssemblyQualifiedName { get => this.GetPropertyValue(this._assemblyQualifiedName); set => this.SetProperty(this._assemblyQualifiedName, this._assemblyQualifiedName = value); }
    }
}