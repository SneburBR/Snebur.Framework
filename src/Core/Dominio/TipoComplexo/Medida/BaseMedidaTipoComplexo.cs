using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using System;
using System.Diagnostics;

namespace Snebur.Dominio
{
    [IgnorarClasseTS]
    [IgnorarGlobalizacao]
    public abstract class BaseMedidaTipoComplexo : BaseTipoComplexo, IDpiVisualizacao
    {
        private Func<double> _funcaoDpiVisualizacao;

        [NaoMapear]
        [IgnorarPropriedadeTSAttribute]
        Func<double> IDpiVisualizacao.FuncaoDpiVisualizacao { get => this._funcaoDpiVisualizacao; set => this._funcaoDpiVisualizacao = value; }

        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        [NaoMapear]
        public double DpiVisualizacao
        {
            get
            {
                if (this._funcaoDpiVisualizacao != null)
                {
                    return this._funcaoDpiVisualizacao.Invoke();
                }
                return 0;
            }
            set
            {
                var retorno = value;
                this._funcaoDpiVisualizacao = () =>
                {
                    return retorno;
                };
            }
        }

        protected int? RetornarValorVisualizacao(double? valor)
        {
            if (valor == null)
            {
                return null;
            }
            return this.RetornarValorVisualizacao(valor.Value);
        }

        protected int RetornarValorVisualizacao(double valor)
        {
            if (this.DpiVisualizacao == 0 && !this.IsSerializando)
            {
                if (DebugUtil.IsAttached)
                {
                    throw new Exception($"O Dpi visualização não foi definido, utilizar o método estático {nameof(MedidaUtil)}.{nameof(MedidaUtil.DefinirDpiVisualizacao)} e passar o bojeto");
                }
            }
            return MedidaUtil.RetornarPixelsVisualizacao(valor, this.DpiVisualizacao);
        }

        internal protected override BaseTipoComplexo BaseClone()
        {
            //var retorno = this.BaseClone();
            //retorno.DpiVisualizacao = this.DpiVisualizacao;
            //return retorno;

            throw new NotImplementedException();
            //retorno.DpiVisualizacao = this.DpiVisualizacao;
        }
        [NaoMapear]
        public abstract bool IsEmpty { get; }
    }
}