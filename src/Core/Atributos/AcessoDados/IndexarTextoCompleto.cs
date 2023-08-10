using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property)]
    public class IndexarTextoCompletoAttribute : Attribute, IAtributoMigracao
    {
        [IgnorarPropriedadeTS, IgnorarPropriedadeTSReflexao]
        public bool IsIgnorarMigracao { get; set; }
    }
}
