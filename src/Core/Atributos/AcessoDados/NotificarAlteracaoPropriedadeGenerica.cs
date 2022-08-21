using System;

namespace Snebur.Dominio.Atributos
{
    //[IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property)]
    public class NotificarAlteracaoPropriedadeGenericaAttribute : BaseAtributoDominio
    {
        public NotificarAlteracaoPropriedadeGenericaAttribute()
        {
        }
    }
    [AttributeUsage(AttributeTargets.Class)]
    public class NotificarAlteracaoEntidadeAttribute : BaseAtributoDominio
    {
        public NotificarAlteracaoEntidadeAttribute()
        {
        }
    }
}