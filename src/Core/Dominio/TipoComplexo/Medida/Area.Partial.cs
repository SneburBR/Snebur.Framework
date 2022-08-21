﻿using Newtonsoft.Json;
using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public partial class Area : BaseMedidaTipoComplexo, IArea
    {
        public static Area Empty => new Area(null, null, null, null, 0, 0);

        [JsonIgnore]
        [NaoMapear]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        public int? EsquerdaVisualizacao { get => this.RetornarValorVisualizacao(this._esquerda); }

        [JsonIgnore]
        [NaoMapear]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        public int? SuperiorVisualizacao { get => this.RetornarValorVisualizacao(this._superior); }

        [JsonIgnore]
        [NaoMapear]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        public int? DireitaVisualizacao { get => this.RetornarValorVisualizacao(this._direita); }

        [JsonIgnore]
        [NaoMapear]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        public int? InferiorVisualizacao { get => this.RetornarValorVisualizacao(this._inferior); }

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
        public Margem Margem
        {
            get => new Margem(this.Esquerda, this.Superior, this.Direita, this.Inferior);
            set
            {
                this.Esquerda = value.Esquerda;
                this.Superior = value.Superior;
                this.Direita = value.Direita;
                this.Inferior = value.Inferior;
            }
        }
        [JsonIgnore]
        [NaoMapear]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        public override bool IsEmpty
        {
            get
            {
                return this.Margem.IsEmpty && this.Dimensao.IsEmpty;
            }
        }
    }
}