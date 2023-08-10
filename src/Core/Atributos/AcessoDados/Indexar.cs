using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property)]
    public class IndexarAttribute : Attribute, IAtributoMigracao
    {
        [IgnorarPropriedadeTS, IgnorarPropriedadeTSReflexao]
        public bool IsIgnorarMigracao { get; set; }
    }

}