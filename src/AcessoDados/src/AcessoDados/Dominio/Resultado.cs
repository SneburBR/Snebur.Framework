using Snebur.AcessoDados.Seguranca;

namespace Snebur.AcessoDados
{
    public abstract class Resultado : BaseAcessoDados
    {

		#region Campos Privados

        private bool _isSucesso;
        private EnumPermissao _permissao;

		#endregion

        public bool IsSucesso { get => this.RetornarValorPropriedade(this._isSucesso); set => this.NotificarValorPropriedadeAlterada(this._isSucesso, this._isSucesso = value); }

        public EnumPermissao Permissao { get => this.RetornarValorPropriedade(this._permissao); set => this.NotificarValorPropriedadeAlterada(this._permissao, this._permissao = value); }
    }
}