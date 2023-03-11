namespace Snebur.Globalizacao
{
    public class Idioma : BaseGlobalizacao
    {
		#region Campos Privados

    	private string _codigo;
        private string _nome;
        private string _nomeNativo;

		#endregion

    	public string Codigo { get => this.RetornarValorPropriedade(this._codigo); set => this.NotificarValorPropriedadeAlterada(this._codigo, this._codigo = value); }

        public string Nome { get => this.RetornarValorPropriedade(this._nome); set => this.NotificarValorPropriedadeAlterada(this._nome, this._nome = value); }

        public string NomeNativo { get => this.RetornarValorPropriedade(this._nomeNativo); set => this.NotificarValorPropriedadeAlterada(this._nomeNativo, this._nomeNativo = value); }

        public DiasSemana DiasSemana { get; set; }

        public Meses Meses { get; set; }
    
    	public Paises Paises { get; set; }
    
    	public TempoSemantico TempoSemantico { get; set; }
    
    	public Cores Cores { get; set; }
    }
}