namespace Snebur.Comunicacao
{
    public abstract class ResultadoChamadaLista : ResultadoChamada
    {
        #region Campos Privados

        private string? _assemblyQualifiedName;

        #endregion

        public string? AssemblyQualifiedName { get => this.RetornarValorPropriedade(this._assemblyQualifiedName); set => this.NotificarValorPropriedadeAlterada(this._assemblyQualifiedName, this._assemblyQualifiedName = value); }
    }
}