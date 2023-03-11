namespace Snebur.Globalizacao
{
    public class FormatacaoFiscal : BaseGlobalizacao
    {
		#region Campos Privados

    	private string _codigoFiscal;
    	private string _codigoFiscalEmpresa;

		#endregion

    	public string CodigoFiscal { get => this.RetornarValorPropriedade(this._codigoFiscal); set => this.NotificarValorPropriedadeAlterada(this._codigoFiscal, this._codigoFiscal = value); }

    	public string CodigoFiscalEmpresa { get => this.RetornarValorPropriedade(this._codigoFiscalEmpresa); set => this.NotificarValorPropriedadeAlterada(this._codigoFiscalEmpresa, this._codigoFiscalEmpresa = value); }
    }
}