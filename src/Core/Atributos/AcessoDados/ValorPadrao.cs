using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property)]
    public class ValorPadraoAttribute : Attribute, IValorPadrao
    {
        public object ValorPadrao { get; set; }

        public ValorPadraoAttribute(Enum valorPadrao)
        {
            this.ValorPadrao = valorPadrao;
        }

        public ValorPadraoAttribute(string valorPadrao)
        {
            this.ValorPadrao = valorPadrao;
        }

        public ValorPadraoAttribute(int valorPadrao)
        {
            this.ValorPadrao = valorPadrao;
        }

        public ValorPadraoAttribute(bool valorPadrao)
        {
            this.ValorPadrao = valorPadrao;
        }

        public ValorPadraoAttribute(DateTime valorPadrao)
        {
            this.ValorPadrao = valorPadrao;
        }

        public ValorPadraoAttribute(decimal valorPadrao)
        {
            this.ValorPadrao = valorPadrao;
        }

        public ValorPadraoAttribute(double valorPadrao)
        {
            this.ValorPadrao = valorPadrao;
        }

        public object RetornarValorPadrao(object contexto, Entidade entidade)
        {
            return this.ValorPadrao;
        }

        #region IValorPadrao 

        public bool TipoNullableRequerido { get { return true; } }

        #endregion
    }
}