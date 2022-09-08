using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Mapeamento
{
    internal class MapeamentoConsultaRelacaoAbertaUmUmReversa : MapeamentoConsultaRelacaoAberta
    {

        internal EstruturaRelacaoUmUmReversa EstruturaRelacaoUmUmReversa
        {
            get
            {
                return (EstruturaRelacaoUmUmReversa)this.EstruturaRelacao;
            }
        }

        internal MapeamentoConsultaRelacaoAbertaUmUmReversa(EstruturaConsulta estruturaConsulta,
                                                            EstruturaBancoDados estruturaBancoDados,
                                                            BaseConexao conexaoDB,
                                                            MapeamentoConsulta mapeamentoConsultaPai,
                                                            EstruturaRelacaoUmUmReversa estruturaRelacaoUmUmReversa,
                                                            BaseRelacaoAberta relacaoAberta,
                                                             BaseContextoDados contexto) :
                                                            base(estruturaConsulta,
                                                                 estruturaBancoDados,
                                                                 conexaoDB,
                                                                 mapeamentoConsultaPai,
                                                                 estruturaRelacaoUmUmReversa,
                                                                 relacaoAberta,
                                                                 contexto)
        {
        }
    }
}