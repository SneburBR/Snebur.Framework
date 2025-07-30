namespace Snebur.Comunicacao.Dominio.RegraNegocio
{
    public class ChamadaRegraNegocio : BaseComunicao
    {

        #region Campos Privados

        private string? _assemblyQualifiedName;
        private string? _nomeMetodo;

        #endregion

        public string? AssemblyQualifiedName { get => this.GetPropertyValue(this._assemblyQualifiedName); set => this.SetProperty(this._assemblyQualifiedName, this._assemblyQualifiedName = value); }

        public string? NomeMetodo { get => this.GetPropertyValue(this._nomeMetodo); set => this.SetProperty(this._nomeMetodo, this._nomeMetodo = value); }
    }
}