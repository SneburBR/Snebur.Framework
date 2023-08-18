using System;

namespace Snebur.Dominio.Atributos
{
    //[IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property)]
    public class NotificarAlteracaoPropriedadeGenericaAttribute : BaseAtributoDominio, INotificarAlteracaoPropriedade
    {
        public EnumOpcoesAlterarPropriedade Opcoes { get; }
        public NotificarAlteracaoPropriedadeGenericaAttribute(EnumOpcoesAlterarPropriedade opcoes = EnumOpcoesAlterarPropriedade.None)
        {
            this.Opcoes = opcoes;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class NotificarTodasAlteracoesPropriedadeGenericaAttribute : BaseAtributoDominio, INotificarAlteracaoPropriedade
    {
        public EnumOpcoesAlterarPropriedade Opcoes { get; }
        public NotificarTodasAlteracoesPropriedadeGenericaAttribute()
        {

        }
    }

    [IgnorarInterfaceTS]
    public interface INotificarAlteracaoPropriedade
    {
        EnumOpcoesAlterarPropriedade Opcoes { get; }
    }
}