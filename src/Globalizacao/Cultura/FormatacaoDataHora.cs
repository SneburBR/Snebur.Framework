namespace Snebur.Globalizacao
{
    public class FormatacaoDataHora : BaseGlobalizacao
    {
		#region Campos Privados

        private string _data;
        private string _hora;
        private string _horaCompleta;

		#endregion

        public string Data { get => this.RetornarValorPropriedade(this._data); set => this.NotificarValorPropriedadeAlterada(this._data, this._data = value); }

        public string Hora { get => this.RetornarValorPropriedade(this._hora); set => this.NotificarValorPropriedadeAlterada(this._hora, this._hora = value); }

        public string HoraCompleta { get => this.RetornarValorPropriedade(this._horaCompleta); set => this.NotificarValorPropriedadeAlterada(this._horaCompleta, this._horaCompleta = value); }
    }
}