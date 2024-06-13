using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class NaoMapearAttribute :
#if NET6_0_OR_GREATER
        Attribute
#else
        NotMappedAttribute
#endif
    {
    }
}
