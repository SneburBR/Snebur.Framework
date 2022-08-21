using Newtonsoft.Json;
using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public partial class Dimensao : BaseMedidaTipoComplexo, IDimensao
    {
        public static Dimensao Empty = new Dimensao(0, 0);

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

        [JsonIgnore]
        [NaoMapear]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        public int LarguraPixels
        {
            get
            {
                if (this.DpiVisualizacao > 0)
                {
                    return this.LarguraVisualizacao;
                }
                return (int)this.Largura;
            }
        }
        [NaoMapear]
        [JsonIgnore]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        public int AlturaPixels
        {
            get
            {
                if (this.DpiVisualizacao > 0)
                {
                    return this.AlturaVisualizacao;
                }
                return (int)this.Altura;
            }
        }
        [NaoMapear]
        [JsonIgnore]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        public bool IsQuadrada => this.Orientacao == EnumOrientacao.Quadrado;

        [NaoMapear]
        [JsonIgnore]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        public override bool IsEmpty => this.Largura == 0 && this.Altura == 0;

        [NaoMapear]
        [JsonIgnore]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        public EnumOrientacao Orientacao
        {
            get
            {
                if (this.Largura == this.Altura)
                {
                    return EnumOrientacao.Quadrado;
                }
                if (this.Largura > this.Altura)
                {
                    return EnumOrientacao.Horizontal;
                }
                return EnumOrientacao.Vertical;
            }
        }
    }
}