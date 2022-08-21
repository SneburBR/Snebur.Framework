using Newtonsoft.Json;
using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public partial class Regiao : BaseMedidaTipoComplexo, IRegiao
    {
        public static Regiao Empty => new Regiao(0, 0, 0, 0);

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
        public int LarguraVisualizacao { get => this.RetornarValorVisualizacao(this._largura); }

        [JsonIgnore]
        [NaoMapear]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        public int AlturaVisualizacao { get => this.RetornarValorVisualizacao(this._altura); }

        [NaoMapear]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        public Posicao Posicao
        {
            get => new Posicao(this.X, this.Y);
            set
            {
                this.X = value.X;
                this.Y = value.Y;
            }
        }
        [JsonIgnore]
        [NaoMapear]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        public Dimensao Dimensao
        {
            get => new Dimensao(this.Largura, this.Altura);
            set
            {
                this.Largura = value.Largura;
                this.Altura = value.Altura;
            }
        }
        [JsonIgnore]
        [NaoMapear]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        public override bool IsEmpty => this.Dimensao.IsEmpty && this.Posicao.IsEmpty;

    }
}