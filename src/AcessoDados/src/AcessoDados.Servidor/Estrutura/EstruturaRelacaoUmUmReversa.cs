namespace Snebur.AcessoDados.Estrutura
{
    internal class EstruturaRelacaoUmUmReversa : EstruturaRelacao
    {

        internal EstruturaEntidade EstruturaEntidadeUmUmReversa { get; set; }

        internal EstruturaCampo EstruturaCampoChaveEstrageiraReversa { get; set; }

        internal EstruturaRelacaoUmUm EstruturaRelacaoUmUm { get; set; }

        internal EstruturaRelacaoUmUmReversa(PropertyInfo propriedade,
                                             EstruturaEntidade estruturaEntidade,
                                             EstruturaEntidade estruturaEntidadeReversa,
                                             EstruturaCampo estruturaCampoChaveEstrageiraReversa) : base(propriedade, estruturaEntidade)
        {
            this.EstruturaEntidadeUmUmReversa = estruturaEntidadeReversa;
            this.EstruturaCampoChaveEstrageiraReversa = estruturaCampoChaveEstrageiraReversa;
        }
    }
}