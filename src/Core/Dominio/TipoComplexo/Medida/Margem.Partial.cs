using Newtonsoft.Json;
using Snebur.Dominio.Atributos;

namespace Snebur.Dominio;

//public class Margem : BaseTipoComplexo<Margem>
public partial class Margem : BaseMedidaTipoComplexo, IMargem
{
    [JsonIgnore]
    [NaoMapear]
    [IgnorarPropriedade]
    [IgnorarPropriedadeTSReflexao]
    public static Margem Empty
        => new Margem(null, null, null, null);

    [JsonIgnore]
    [NaoMapear]
    [IgnorarPropriedade]
    [IgnorarPropriedadeTSReflexao]
    public double MaiorValor
        => Math.Max(this.Esquerda ?? 0, Math.Max(this.Superior ?? 0, Math.Max(this.Direita ?? 0, this.Inferior ?? 0)));

    [JsonIgnore]
    [NaoMapear]
    [IgnorarPropriedade]
    [IgnorarPropriedadeTSReflexao]
    public double MenorValor
        => Math.Min(this.Esquerda ?? 0, Math.Min(this.Superior ?? 0, Math.Min(this.Direita ?? 0, this.Inferior ?? 0)));

    [JsonIgnore]
    [NaoMapear]
    [IgnorarPropriedade]
    [IgnorarPropriedadeTSReflexao]
    public double ValorMedio
        => (this.Esquerda ?? 0 + this.Superior ?? 0 + this.Direita ?? 0 + this.Inferior ?? 0) / 4;

    [JsonIgnore]
    [NaoMapear]
    [IgnorarPropriedade]
    [IgnorarPropriedadeTSReflexao]
    public double Vertical
        => Math.Min((this.Superior ?? 0), (this.Inferior ?? 0));

    [JsonIgnore]
    [NaoMapear]
    [IgnorarPropriedade]
    [IgnorarPropriedadeTSReflexao]
    public double Horizontal
        => Math.Min((this.Esquerda ?? 0), (this.Direita ?? 0));

    [JsonIgnore]
    [NaoMapear]
    [IgnorarPropriedade]
    [IgnorarPropriedadeTSReflexao]
    public int? EsquerdaVisualizacao
        => this.RetornarValorVisualizacao(this._esquerda);

    [JsonIgnore]
    [NaoMapear]
    [IgnorarPropriedade]
    [IgnorarPropriedadeTSReflexao]
    public int? SuperiorVisualizacao
        => this.RetornarValorVisualizacao(this._superior);

    [JsonIgnore]
    [NaoMapear]
    [IgnorarPropriedade]
    [IgnorarPropriedadeTSReflexao]
    public int? DireitaVisualizacao
        => this.RetornarValorVisualizacao(this._direita);

    [JsonIgnore]
    [NaoMapear]
    [IgnorarPropriedade]
    [IgnorarPropriedadeTSReflexao]
    public int? InferiorVisualizacao
        => this.RetornarValorVisualizacao(this._inferior);

    [JsonIgnore]
    [NaoMapear]
    [IgnorarPropriedade]
    [IgnorarPropriedadeTSReflexao]
    public bool IsExisteMargem
        => this.Esquerda != null ||
           this.Direita != null ||
           this.Superior != null ||
           this.Inferior != null;

    [JsonIgnore]
    [NaoMapear]
    [IgnorarPropriedade]
    [IgnorarPropriedadeTSReflexao]
    public override bool IsEmpty
        => this.Esquerda == null &&
           this.Direita == null &&
           this.Superior == null &&
           this.Inferior == null;
}