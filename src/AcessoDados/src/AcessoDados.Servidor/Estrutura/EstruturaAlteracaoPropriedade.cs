using Snebur.Dominio.Atributos;
using System.Reflection;

namespace Snebur.AcessoDados.Estrutura
{
    internal class EstruturaAlteracaoPropriedade : EstruturaPropriedade
    {
        internal NotificarAlteracaoPropriedadeAttribute Atributo { get; set; }
        internal EstruturaCampo EstruturaCampo { get; set; }

        internal EstruturaTipoComplexo EstruturaTipoComplexo { get; set; }
        public bool IsTipoComplexo { get; }

        internal EstruturaAlteracaoPropriedade(PropertyInfo propriedade,
                                               EstruturaEntidade estruturaEntidade,
                                               EstruturaCampo estruturaCampo,
                                               NotificarAlteracaoPropriedadeAttribute atributo) :
                                               base(propriedade, estruturaEntidade)
        {
            this.EstruturaCampo = estruturaCampo;
            this.Atributo = atributo;


        }

        internal EstruturaAlteracaoPropriedade(PropertyInfo propriedade,
                                             EstruturaEntidade estruturaEntidade,
                                             EstruturaTipoComplexo estrturaTipoComplexo,
                                             NotificarAlteracaoPropriedadeAttribute atributo) :
                                             base(propriedade, estruturaEntidade)
        {
            this.Atributo = atributo;
            this.EstruturaTipoComplexo = estrturaTipoComplexo;
            this.IsTipoComplexo = true;
        }
    }
}