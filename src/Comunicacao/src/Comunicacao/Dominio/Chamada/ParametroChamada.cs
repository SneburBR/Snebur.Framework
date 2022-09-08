using Snebur.Dominio.Atributos;

namespace Snebur.Comunicacao
{
    [Plural("ParametrosChamada")]
    public abstract class ParametroChamada : BaseComunicao
    {
        #region Campos Privados

        private string _nome;
        private string _nomeTipoParametro;
        private string _assemblyQualifiedName;

        #endregion

        public string Nome { get => this.RetornarValorPropriedade(this._nome); set => this.NotificarValorPropriedadeAlterada(this._nome, this._nome = value); }

        public string NomeTipoParametro { get => this.RetornarValorPropriedade(this._nomeTipoParametro); set => this.NotificarValorPropriedadeAlterada(this._nomeTipoParametro, this._nomeTipoParametro = value); }

        public string AssemblyQualifiedName { get => this.RetornarValorPropriedade(this._assemblyQualifiedName); set => this.NotificarValorPropriedadeAlterada(this._assemblyQualifiedName, this._assemblyQualifiedName = value); }
    }
}