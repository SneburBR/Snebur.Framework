namespace Snebur.Comunicacao.Dominio.Resultado
{
    public class ResultadoChamadaEnum : ResultadoChamada
    {
        #region Campos Privados

        private string? _nomeTipoEnum;
        private string? _namespaceEnum;
        private int _valor;

        #endregion

        public string? NomeTipoEnum { get => this.RetornarValorPropriedade(this._nomeTipoEnum); set => this.NotificarValorPropriedadeAlterada(this._nomeTipoEnum, this._nomeTipoEnum = value); }

        public string? NamespaceEnum { get => this.RetornarValorPropriedade(this._namespaceEnum); set => this.NotificarValorPropriedadeAlterada(this._namespaceEnum, this._namespaceEnum = value); }

        public int Valor { get => this.RetornarValorPropriedade(this._valor); set => this.NotificarValorPropriedadeAlterada(this._valor, this._valor = value); }
    }
}