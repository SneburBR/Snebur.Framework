using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class NaoMapearAttribute :
#if NET7_0
        Attribute
#else
        NotMappedAttribute
#endif
    {
    }
}
