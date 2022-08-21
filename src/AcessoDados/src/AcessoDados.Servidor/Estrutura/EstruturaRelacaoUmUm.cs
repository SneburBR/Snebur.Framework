using System.Reflection;

namespace Snebur.AcessoDados.Estrutura
{
    internal class EstruturaRelacaoUmUm : EstruturaRelacaoChaveEstrangeira
    {

        internal EstruturaRelacaoUmUm(PropertyInfo propriedade,
                                      EstruturaEntidade estruturaEntidade,
                                      EstruturaEntidade estruturaEntidadeUmUm,
                                      EstruturaCampo estruturaCampoChaveEstrangeira) : base(propriedade, estruturaEntidade, estruturaEntidadeUmUm, estruturaCampoChaveEstrangeira)
             
        {
            //this.EstruturaEntidadeRelavaoUmUm = estruturaEntidadeUmUm;
            //this.EstruturaCampoChaveEstrangeira = estruturaCampoChaveEstrangeira;
            //this.Requerido = AjudanteEstruturaBancoDados.PropriedadeRequerida(this.Propriedade);
            //this.EstruturaCampoChaveEstrangeira.EstruturaRelacaoUmUm = this;
        }
    }
}