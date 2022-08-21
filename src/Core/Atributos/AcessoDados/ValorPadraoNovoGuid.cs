using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValorPadraoNovoGuidAttribute : Attribute, IValorPadrao
    {
        public object RetornarValorPadrao(object contexto, Entidade entidade)
        {
            return Guid.NewGuid();
        }

        #region IValorPadrao 

        public bool TipoNullableRequerido { get { return false; } }

        #endregion
    }
}