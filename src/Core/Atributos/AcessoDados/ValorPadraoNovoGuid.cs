using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValorPadraoNovoGuidAttribute : Attribute, IValorPadrao
    {
        public bool IsValorPadraoOnUpdate { get; } = false;
        public object RetornarValorPadrao(object contexto, 
                                         Entidade entidade, 
                                         object valorPropriedade)
        {
            return Guid.NewGuid();
        }

        #region IValorPadrao 

        public bool IsTipoNullableRequerido { get { return false; } }

        #endregion
    }
}