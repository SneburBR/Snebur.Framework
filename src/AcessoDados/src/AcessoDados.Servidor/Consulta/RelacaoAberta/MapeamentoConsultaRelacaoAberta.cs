using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Mapeamento
{
    internal abstract class MapeamentoConsultaRelacaoAberta :MapeamentoConsulta
    {
        internal protected EstruturaRelacao EstruturaRelacao { get; set; }
        internal protected BaseRelacaoAberta RelacaoAberta { get; set; }
        internal protected MapeamentoConsulta MapeamentoConsultaPai { get; set; }

        internal MapeamentoConsultaRelacaoAberta(EstruturaConsulta estruturaConsulta,
                                                 EstruturaBancoDados estruturaBancoDados,
                                                 BaseConexao conexaoDB,
                                                 MapeamentoConsulta mapeamentoConsultaPai,
                                                 EstruturaRelacao estruturaReleacao,
                                                 BaseRelacaoAberta relacaoAberta, BaseContextoDados contexto) : base(estruturaConsulta,
                                                                                        estruturaBancoDados,
                                                                                        conexaoDB, contexto)
        {
            this.MapeamentoConsultaPai = mapeamentoConsultaPai;
            this.EstruturaRelacao = estruturaReleacao;
            this.RelacaoAberta = relacaoAberta;
        }
    }
}