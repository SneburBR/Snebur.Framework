using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Mapeamento
{
    internal class MapeamentoConsultaSql<T> : BaseMapeamentoConsulta
    {

        internal MapeamentoConsultaSql(EstruturaConsulta estruturaConsulta,
                                       EstruturaBancoDados estruturaBancoDados,
                                       BaseConexao ConexaoDB, BaseContextoDados contexto) :
            base(estruturaConsulta, estruturaBancoDados, ConexaoDB, contexto)
        {
        }

        internal T RetornarConsultaSql()
        {
            throw new NotImplementedException();
        }
    }
}
