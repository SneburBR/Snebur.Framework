using Snebur.Dominio.Atributos;
using System.Reflection;

namespace Snebur.AcessoDados.Estrutura
{
    internal class EstruturaAlteracaoPropriedadeGenerica : EstruturaPropriedade
    {
        internal NotificarAlteracaoPropriedadeGenericaAttribute Atributo { get; set; }
        internal EstruturaCampo EstruturaCampo { get; set; }

        internal EstruturaAlteracaoPropriedadeGenerica(PropertyInfo propriedade,
                                                       EstruturaEntidade estruturaEntidade,
                                                       EstruturaCampo estruturaCampo,
                                                       NotificarAlteracaoPropriedadeGenericaAttribute atributo) :
                                                       base(propriedade, estruturaEntidade)
        {
            this.EstruturaCampo = estruturaCampo;
            this.Atributo = atributo;
        }
    }
}