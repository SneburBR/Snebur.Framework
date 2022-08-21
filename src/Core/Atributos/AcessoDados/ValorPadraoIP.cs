using Snebur.Utilidade;
using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValorPadraoIPAttribute : SomenteLeituraAttribute, IValorPadrao
    {

        public ValorPadraoIPAttribute()
        {
        }

        public object RetornarValorPadrao(object contexto, Entidade entidade)
        {
            return IpUtil.RetornarIpInternet();
        }
        #region IValorPadrao 

        public bool TipoNullableRequerido { get { return true; } }

        #endregion
    }
}