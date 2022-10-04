﻿using Snebur.Utilidade;
using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValorPadraoIPAttribute : SomenteLeituraAttribute, IValorPadrao
    {

        public ValorPadraoIPAttribute()
        {
        }

        public object RetornarValorPadrao(object contexto, 
                                          Entidade entidade, 
                                          object valorPropriedade)
        {
            return IpUtil.RetornarIpInternet();
        }
        #region IValorPadrao 

        public bool IsTipoNullableRequerido { get { return true; } }

        #endregion
    }
}