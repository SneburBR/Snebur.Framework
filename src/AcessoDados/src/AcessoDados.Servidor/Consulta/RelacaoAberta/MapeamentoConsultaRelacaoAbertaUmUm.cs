using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Mapeamento;

internal class MapeamentoConsultaRelacaoAbertaUmUm : MapeamentoConsultaRelacaoAberta
{

    internal EstruturaRelacaoUmUm EstruturaRelacaoUmUm
    {
        get
        {
            return (EstruturaRelacaoUmUm)this.EstruturaRelacao;
        }
    }

    internal MapeamentoConsultaRelacaoAbertaUmUm(EstruturaConsulta estruturaConsulta,
                                                 EstruturaBancoDados estruturaBancoDados,
                                                 BaseConexao conexaoDB,
                                                 MapeamentoConsulta mapeamentoConsultaPai,
                                                 EstruturaRelacaoUmUm estruturaRelacaoUmUm,
                                                 BaseRelacaoAberta relacaoAberta,
                                                 BaseContextoDados contexto) :
                                                 base(estruturaConsulta,
                                                      estruturaBancoDados,
                                                      conexaoDB,
                                                      mapeamentoConsultaPai,
                                                      estruturaRelacaoUmUm,
                                                      relacaoAberta,
                                                      contexto)
    {           
    }
}