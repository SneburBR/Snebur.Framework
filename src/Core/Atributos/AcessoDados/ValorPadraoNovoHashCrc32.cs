using Snebur.Utilidade;
using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValorPadraoNovoHashCrc32 : Attribute, IValorPadrao
    {
        public object RetornarValorPadrao(object contexto, Entidade entidade)
        {
            return ChecksumUtil.RetornarChecksumCrc2(Guid.NewGuid().ToByteArray());
        }

        #region IValorPadrao 

        public bool TipoNullableRequerido { get { return false; } }

        #endregion
    }
}