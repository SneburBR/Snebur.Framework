using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property)]
    public class ValorPadraoAttribute : Attribute, IValorPadrao
    {
        public object ValorPadrao { get; set; }
        public bool IsValorPadraoOnUpdate { get; set; }

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

        public object RetornarValorPadrao(object contexto, 
                                          Entidade entidade,
                                          object valorPropriedade)
        {
            return this.ValorPadrao;
        }

        #region IValorPadrao 

        public bool IsTipoNullableRequerido { get { return true; } }

        #endregion
    }
}