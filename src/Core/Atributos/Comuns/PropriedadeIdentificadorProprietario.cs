using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropriedadeIdentificadorProprietarioAttribute : BaseAtributoDominio, IBaseValorPadrao
    {
        public bool IsTipoNullableRequerido => false;

        public bool IsValorPadraoOnUpdate => false;
    }
}