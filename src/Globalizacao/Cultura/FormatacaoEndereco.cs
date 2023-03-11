namespace Snebur.Globalizacao
{
    public class FormatacaoEndereco : BaseGlobalizacao
    {
		#region Campos Privados

        private string _codigoPostal;

		#endregion

        public string CodigoPostal { get => this.RetornarValorPropriedade(this._codigoPostal); set => this.NotificarValorPropriedadeAlterada(this._codigoPostal, this._codigoPostal = value); }
    }
}