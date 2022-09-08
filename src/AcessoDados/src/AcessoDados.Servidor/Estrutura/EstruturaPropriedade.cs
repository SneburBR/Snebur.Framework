using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Snebur.AcessoDados.Estrutura
{
    internal abstract class EstruturaPropriedade
    {
        internal Type Tipo { get; }

        internal protected PropertyInfo Propriedade { get; protected set; }

        internal bool Requerido { get; set; }

        internal bool IsAceitaNulo { get; set; }

        internal bool IsTipoNullable { get; }

        internal EstruturaEntidade EstruturaEntidade { get; }

        internal List<string> Alertas = new List<string>();

        internal EstruturaPropriedade(PropertyInfo propriedade, EstruturaEntidade estruturaEntidade)
        {
            this.EstruturaEntidade = estruturaEntidade;
            this.Propriedade = propriedade;
            this.Tipo = propriedade.PropertyType;
            this.Requerido = AjudanteEstruturaBancoDados.PropriedadeRequerida(this.Propriedade);
            this.IsTipoNullable = ReflexaoUtil.IsTipoNullable(this.Propriedade.PropertyType);
            this.IsAceitaNulo = this.RetornarAceitarNulo();
        }

        private bool RetornarAceitarNulo()
        {
            if (ValidacaoUtil.IsPropriedadeRequerida(this.Propriedade))
            {
                return false;
            }
            return !this.Propriedade.PropertyType.IsValueType || this.IsTipoNullable;
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}", this.Propriedade.Name, base.ToString());
        }
    }
}