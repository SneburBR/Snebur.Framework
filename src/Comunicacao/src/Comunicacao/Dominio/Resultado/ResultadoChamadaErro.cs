using Snebur.Dominio.Atributos;

namespace Snebur.Comunicacao;

public abstract class ResultadoChamadaErro : ResultadoChamada
{

    #region Campos Privados

    private int _statusCode;
    private string? _mensagemErro;
    private object? _erro;

    #endregion

    [CampoProtegido]
    public string? MensagemErro { get => this.GetPropertyValue(this._mensagemErro); set => this.SetProperty(this._mensagemErro, this._mensagemErro = value); }
    public object? Erro { get => this.GetPropertyValue(this._erro); set => this.SetProperty(this._erro, this._erro = value); }
    public int StatusCode { get => this.GetPropertyValue(this._statusCode); set => this.SetProperty(this._statusCode, this._statusCode = value); }
}