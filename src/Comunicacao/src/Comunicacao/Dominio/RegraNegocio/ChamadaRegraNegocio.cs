namespace Snebur.Comunicacao
{
    public class ChamadaRegraNegocio : BaseComunicao
    {

		#region Campos Privados

        private string _assemblyQualifiedName;
        private string _nomeMetodo;

		#endregion

        public string AssemblyQualifiedName { get => this.RetornarValorPropriedade(this._assemblyQualifiedName); set => this.NotificarValorPropriedadeAlterada(this._assemblyQualifiedName, this._assemblyQualifiedName = value); }

        public string NomeMetodo { get => this.RetornarValorPropriedade(this._nomeMetodo); set => this.NotificarValorPropriedadeAlterada(this._nomeMetodo, this._nomeMetodo = value); }

        public ChamadaRegraNegocio()
        {
        }
    }
}