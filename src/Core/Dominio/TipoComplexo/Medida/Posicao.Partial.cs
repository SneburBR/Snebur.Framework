using Newtonsoft.Json;
using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public partial class Posicao : BaseMedidaTipoComplexo, IPosicao
    {
        public static Posicao Empty => new Posicao(0, 0);

        [JsonIgnore]
        [NaoMapear]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        public int XVisualizacao { get => this.RetornarValorVisualizacao(this._x); }

        [JsonIgnore]
        [NaoMapear]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        public int YVisualizacao { get => this.RetornarValorVisualizacao(this._y); }

        [JsonIgnore]
        [NaoMapear]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        public override bool IsEmpty => this.X == 0 && this.Y == 0;
    }
}
