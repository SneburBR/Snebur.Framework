using Snebur.Dominio.Atributos;
using Snebur.Utilidade;

namespace Snebur.Dominio
{
    [IgnorarClasseTS]

    public partial class Dimensao : BaseMedidaTipoComplexo, IDimensao
    {
        private double _largura;
        private double _altura;

        [ValidacaoRequerido]
        public double Largura { get => this._largura; set => this.NotificarValorPropriedadeAlterada(this._largura, this._largura = value); }

        [ValidacaoRequerido]
        public double Altura { get => this._altura; set => this.NotificarValorPropriedadeAlterada(this._altura, this._altura = value); }

        public Dimensao()
        {
        }
        [IgnorarConstrutorTS]
        public Dimensao(double largura, double altura)
        {
            this._largura = largura;
            this._altura = altura;
        }
        [IgnorarConstrutorTS]
        public Dimensao(double largura, double altura, double dpiVisualizacao) : this(largura, altura)
        {
            this.DpiVisualizacao = dpiVisualizacao;
        }

        public override string ToString()
        {
            return base.ToString() + $" {this.Largura} x {this.Altura}";
        }

        public override bool Equals(object obj)
        {
            if (obj is Dimensao d)
            {
                return d.Largura == this.Largura && d.Altura == this.Altura;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            //Problemas ao definir o dpi de impressão
            return base.GetHashCode();
            //return this.Largura.GetHashCode() + this.Altura.GetHashCode();
        }
        //public override Dimensao Clone()
        //{
        //    throw new System.NotImplementedException();
        //}

        //public static Tamanho Empty
        //{
        //    get
        //    {
        //        return new Tamanho(0, 0);
        //    }
        //}

        protected internal override BaseTipoComplexo BaseClone()
        {
            return new Dimensao(this.Largura, this.Altura);
        }

        public Dimensao Escalar(double scalar)
        {
            return new Dimensao(this.Largura * scalar,
                                this.Altura * scalar);
        }

        public Dimensao ParaPixels(double dpi)
        {
            var largura = MedidaUtil.ParaPixels(this.Largura, dpi);
            var altura = MedidaUtil.ParaPixels(this.Altura, dpi);
            return new Dimensao(largura, altura);
        }

        public bool Menor(Dimensao dimensao)
        {
            return this.Largura < dimensao.Largura ||
              this.Altura < dimensao.Altura;
        }
    }
}