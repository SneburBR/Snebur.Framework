using Snebur.UI;
using System;

namespace Snebur.Dominio.Atributos
{
    //[IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property)]
    public class NotificarAlteracaoPropriedadeGenericaAttribute : BaseAtributoDominio, INotificarAlteracaoPropriedade
    {
        public EnumOpcoesAlterarPropriedade Opcoes { get; }
        public string Formatacao { get; set; }
        public Type TipoPropriedade { get; set; }
        //public NotificarAlteracaoPropriedadeGenericaAttribute(EnumOpcoesAlterarPropriedade opcoes = EnumOpcoesAlterarPropriedade.None)
        //{
        //    this.TipoPropriedade = null;
        //    this.Opcoes = opcoes;
        //}

        //[IgnorarConstrutorTS]
        public NotificarAlteracaoPropriedadeGenericaAttribute([TipoTS("string")] EnumFormatacao formatacao = EnumFormatacao.Nenhuma, 
                                                              Type tipoPropriedade = null,
                                                              EnumOpcoesAlterarPropriedade opcoes = EnumOpcoesAlterarPropriedade.Nenhuma)
        {
            this.TipoPropriedade = tipoPropriedade;
            this.Formatacao = formatacao.ToString().ToLower();
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