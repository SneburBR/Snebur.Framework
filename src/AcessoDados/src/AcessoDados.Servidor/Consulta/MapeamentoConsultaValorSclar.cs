using Snebur.AcessoDados.Estrutura;
using System.Linq;

namespace Snebur.AcessoDados.Mapeamento
{
    internal class MapeamentoConsultaValorScalar : BaseMapeamentoConsulta
    {

        internal MapeamentoConsultaValorScalar(EstruturaConsulta estruturaConsulta,
                                               EstruturaBancoDados estruturaBancoDados,
                                               BaseConexao ConexaoDB,
                                               BaseContextoDados contexto) :
                                               base(estruturaConsulta, estruturaBancoDados, ConexaoDB, contexto)
        {
        }

        internal object RetornarValorScalar()
        {
            var tipoEntidade = this.EstruturaConsulta.RetornarTipoEntidade();
            var estruturaEntidade = this.EstruturaBancoDados.EstruturasEntidade[tipoEntidade.Name];

            using (var mapeamento = new MapeamentoEntidadeValorScalar(this, 
                                                                      estruturaEntidade, 
                                                                      this.EstruturaBancoDados, 
                                                                      this.ConexaoDB,
                                                                      this.Contexto))
            {
                mapeamento.EstruturaConsulta.Take = 1;
                var sqlValorScalar = mapeamento.RetornarSql( new FiltroMapeamentoVazio(), isIncluirOrdenacaoPaginacao: false);
                return this.ConexaoDB.RetornarValorScalar(sqlValorScalar,
                                                          mapeamento.ParametrosInfo);
            }
        }
    }
}