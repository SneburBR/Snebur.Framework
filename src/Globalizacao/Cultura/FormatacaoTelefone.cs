namespace Snebur.Globalizacao
{
    public class FormatacaoTelefone : BaseGlobalizacao
    {
		#region Campos Privados

    	private string _formatacao;
    	private string _formatacao2;

		#endregion

    	public string Formatacao { get => this.RetornarValorPropriedade(this._formatacao); set => this.NotificarValorPropriedadeAlterada(this._formatacao, this._formatacao = value); }

    	public string Formatacao2 { get => this.RetornarValorPropriedade(this._formatacao2); set => this.NotificarValorPropriedadeAlterada(this._formatacao2, this._formatacao2 = value); }
    }
}