namespace Snebur.AcessoDados.Estrutura;

internal abstract class EstruturaRelacaoChaveEstrangeira : EstruturaRelacao
{
    internal EstruturaEntidade EstruturaEntidadeChaveEstrangeiraDeclarada { get; set; }

    internal EstruturaCampo EstruturaCampoChaveEstrangeira { get; set; }

    internal EstruturaRelacaoChaveEstrangeira(PropertyInfo propriedade,
                                              EstruturaEntidade estruturaEntidade,
                                              EstruturaEntidade estruturaEntidadeChaveEstrangeira,
                                              EstruturaCampo estrutaCampoChaveEstrangeira) :
                                              base(propriedade, estruturaEntidade)
    {
        this.EstruturaCampoChaveEstrangeira = estrutaCampoChaveEstrangeira;
        this.IsRequerido = AjudanteEstruturaBancoDados.PropriedadeRequerida(this.Propriedade);
        this.EstruturaEntidadeChaveEstrangeiraDeclarada = estruturaEntidadeChaveEstrangeira;
        this.EstruturaCampoChaveEstrangeira.EstruturaRelacaoChaveEstrangeira = this;
    }
}