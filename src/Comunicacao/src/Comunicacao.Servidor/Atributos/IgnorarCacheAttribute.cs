using Snebur.Dominio.Atributos;
using System;

namespace Snebur.Comunicacao
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Method)]
    public class IgnorarCacheAttribute : Attribute
    {
    }
}
