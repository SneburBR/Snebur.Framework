using Newtonsoft.Json;
using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public partial class Borda : BaseMedidaTipoComplexo, IBorda
    {
        [NaoMapear]
        [JsonIgnore]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        public Cor Cor { get => new Cor(this.CorRgba); set => this.CorRgba = value.Rgba; }

        [NaoMapear]
        [JsonIgnore]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        public double AfastamentoVisualizacao { get => this.RetornarValorVisualizacao(this._afastamento); }

        [NaoMapear]
        [JsonIgnore]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        public double EspessuraVisualizacao { get => this.RetornarValorVisualizacao(this._espessura); }

        [NaoMapear]
        [JsonIgnore]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        public bool IsExisteBorda
        {
            get
            {
                return !this.Cor.IsTransparente || this.Arredondamento > 0;
            }
        }
        [NaoMapear]
        [JsonIgnore]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        public override bool IsEmpty
        {
            get
            {
                return this.Cor.IsTransparente && this.Espessura == 0 && this.Afastamento == 0;
            }
        }
    }
}