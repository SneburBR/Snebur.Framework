using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Mapeamento;

internal class MapeamentoConsultaRelacaoAbertaPai : MapeamentoConsultaRelacaoAberta
{

    internal EstruturaRelacaoPai EstruturaRelacaoPai
    {
        get
        {
            return (EstruturaRelacaoPai)this.EstruturaRelacao;
        }
    }

    internal MapeamentoConsultaRelacaoAbertaPai(EstruturaConsulta estruturaConsulta,
                                                EstruturaBancoDados estruturaBancoDados,
                                                BaseConexao conexaoDB,
                                                MapeamentoConsulta mapeamentoConsultaPai,
                                                EstruturaRelacaoPai estruturaRelacaoPai,
                                                BaseRelacaoAberta relacaoAberta,
                                               BaseContextoDados contexto) :
                                                base(estruturaConsulta,
                                                     estruturaBancoDados,
                                                     conexaoDB,
                                                     mapeamentoConsultaPai,
                                                     estruturaRelacaoPai,
                                                     relacaoAberta,
                                                     contexto)
    {
    }
}