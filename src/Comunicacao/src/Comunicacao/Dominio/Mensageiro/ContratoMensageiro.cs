namespace Snebur.Comunicacao;

public class ContratoMensageiro : BaseDominio
{

    #region Campos Privados

    private string? _nomeRecurso;

    #endregion

    public BaseDominio? Remetente { get; set; }

    public BaseDominio? Destinatario { get; set; }

    public string? NomeRecurso { get => this.GetPropertyValue(this._nomeRecurso); set => this.SetProperty(this._nomeRecurso, this._nomeRecurso = value); }

    public BaseDominio? ValorParametro { get; set; }
}