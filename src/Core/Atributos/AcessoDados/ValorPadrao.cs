using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property)]
    public class ValorPadraoAttribute : Attribute, IValorPadrao
    {
        private bool _isValorPadraoOnUpdate;
        private EnumTipoValorPadrao _tipoValorPadrao = EnumTipoValorPadrao.Comum;

        public object ValorPadrao { get; set; }
        public bool IsValorPadraoQuandoNull { get; set; }
        public EnumTipoValorPadrao TipoValorPadrao
        {
            get => this.IsValorPadraoQuandoNull ? EnumTipoValorPadrao.ValorPropriedadeNull : _tipoValorPadrao;
            set => _tipoValorPadrao = value;
        }
         
        public bool IsValorPadraoOnUpdate
        {
            get => this.IsValorPadraoQuandoNull || this._isValorPadraoOnUpdate;
            set => this._isValorPadraoOnUpdate = value;
        }
   

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