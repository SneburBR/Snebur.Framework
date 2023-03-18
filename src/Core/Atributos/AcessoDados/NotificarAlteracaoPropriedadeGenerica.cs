using System;

namespace Snebur.Dominio.Atributos
{
    //[IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property)]
    public class NotificarAlteracaoPropriedadeGenericaAttribute : BaseAtributoDominio, INotificarAlteracaoPropriedade
    {
        public EnunFlagAlteracaoPropriedade Flags { get; }
        public NotificarAlteracaoPropriedadeGenericaAttribute(EnunFlagAlteracaoPropriedade flags = EnunFlagAlteracaoPropriedade.None)
        {
            this.Flags = flags;
        }
    }
    [AttributeUsage(AttributeTargets.Class)]
    public class NotificarAlteracaoEntidadeAttribute : BaseAtributoDominio
    {
        public NotificarAlteracaoEntidadeAttribute()
        {
        }
    }

    [IgnorarInterfaceTS]
    public interface INotificarAlteracaoPropriedade
    {
        EnunFlagAlteracaoPropriedade Flags { get; }
    }
}