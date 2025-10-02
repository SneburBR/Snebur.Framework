using Snebur.Dominio.Atributos;

namespace Snebur.Comunicacao;

[Plural("ContratoChamada")]
public class ContratoChamada : BaseComunicao
{

    public Cabecalho? Cabecalho { get; set; }

    public InformacaoSessao? InformacaoSessao { get; set; }
    public Guid IdentificadorSessaoUsuario { get; set; }

    public string? Operacao { get; set; }

    public DateTime DataHora { get; set; }

    public bool Async { get; set; }

    public List<ParametroChamada> Parametros { get; set; } = new();

}