namespace Snebur.Globalizacao
{
    public class TempoSemantico : BaseGlobalizacao
    {
		#region Campos Privados

    	private string _agoraMesmmo;
    	private string _minuto;
    	private string _minutos;
    	private string _hora;
    	private string _horas;
    	private string _dia;
    	private string _dias;
    	private string _ontem;
    	private string _amanha;

		#endregion

    	public string AgoraMesmmo { get => this.RetornarValorPropriedade(this._agoraMesmmo); set => this.NotificarValorPropriedadeAlterada(this._agoraMesmmo, this._agoraMesmmo = value); }
    
    	public string Minuto { get => this.RetornarValorPropriedade(this._minuto); set => this.NotificarValorPropriedadeAlterada(this._minuto, this._minuto = value); }
    
    	public string Minutos { get => this.RetornarValorPropriedade(this._minutos); set => this.NotificarValorPropriedadeAlterada(this._minutos, this._minutos = value); }
    
    	public string Hora { get => this.RetornarValorPropriedade(this._hora); set => this.NotificarValorPropriedadeAlterada(this._hora, this._hora = value); }
    
    	public string Horas { get => this.RetornarValorPropriedade(this._horas); set => this.NotificarValorPropriedadeAlterada(this._horas, this._horas = value); }
    
    	public string Dia { get => this.RetornarValorPropriedade(this._dia); set => this.NotificarValorPropriedadeAlterada(this._dia, this._dia = value); }
    
    	public string Dias { get => this.RetornarValorPropriedade(this._dias); set => this.NotificarValorPropriedadeAlterada(this._dias, this._dias = value); }
    
    	public string Ontem { get => this.RetornarValorPropriedade(this._ontem); set => this.NotificarValorPropriedadeAlterada(this._ontem, this._ontem = value); }
    
    	public string Amanha { get => this.RetornarValorPropriedade(this._amanha); set => this.NotificarValorPropriedadeAlterada(this._amanha, this._amanha = value); }
    }
}