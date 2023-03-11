namespace Snebur.Globalizacao
{
    public class Cultura : BaseGlobalizacao
    {
		#region Campos Privados

    	private string _codigo;
    	private string _codigoIdioma;

		#endregion

    	//https://msdn.microsoft.com/en-us/library/ms533052(v=vs.85).aspx
    
    	public string Codigo { get => this.RetornarValorPropriedade(this._codigo); set => this.NotificarValorPropriedadeAlterada(this._codigo, this._codigo = value); }
    
    	public string CodigoIdioma { get => this.RetornarValorPropriedade(this._codigoIdioma); set => this.NotificarValorPropriedadeAlterada(this._codigoIdioma, this._codigoIdioma = value); }
    
    	public Idioma Idioma { get; set; }
    
    	public FormatacaoMoeda FormatacaoMoeda { get; set; }
    
    	public FormatacaoDataHora FormatacaoDataHora { get; set; }
    
    	public FormatacaoTelefone FormatacaoTelefone { get; set; }
    
    	public FormatacaoEndereco FormatacaoEndereco { get; set; }
    
    	public FormatacaoFiscal FormatacaoFiscal { get; set; }
    
    	public Pais Pais { get; set; }
    }
}