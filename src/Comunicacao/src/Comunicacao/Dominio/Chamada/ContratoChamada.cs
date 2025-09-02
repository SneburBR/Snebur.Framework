using Snebur.Dominio.Atributos;

namespace Snebur.Comunicacao.Dominio;

[Plural("ContratoChamada")]
public class ContratoChamada : BaseComunicao
{
    #region Campos Privados

    private Guid _identificadorSessaoUsuario;
    private string? _operacao;
    private DateTime _dataHora;
    private bool _async;

    #endregion

    public Cabecalho? Cabecalho { get; set; }

    public InformacaoSessao? InformacaoSessao { get; set; }
    public Guid IdentificadorSessaoUsuario { get => this.GetPropertyValue(this._identificadorSessaoUsuario); set => this.SetProperty(this._identificadorSessaoUsuario, this._identificadorSessaoUsuario = value); }

    public string? Operacao { get => this.GetPropertyValue(this._operacao); set => this.SetProperty(this._operacao, this._operacao = value); }

    public DateTime DataHora { get => this.GetPropertyValue(this._dataHora); set => this.SetProperty(this._dataHora, this._dataHora = value); }

    public bool Async { get => this.GetPropertyValue(this._async); set => this.SetProperty(this._async, this._async = value); }

    public List<ParametroChamada> Parametros { get; set; } = new List<ParametroChamada>();

    public ContratoChamada()
    {

    }
}