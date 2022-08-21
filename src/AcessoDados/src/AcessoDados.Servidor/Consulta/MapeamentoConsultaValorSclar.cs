using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using Snebur.AcessoDados.Dominio;
using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Mapeamento
{
    internal class MapeamentoConsultaValorSaclar : BaseMapeamentoConsulta
    {

        internal MapeamentoConsultaValorSaclar(EstruturaConsulta estruturaConsulta,
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

            using (var mapeamento = new MapeamentoEntidadeValorScalar(this, estruturaEntidade, this.EstruturaBancoDados, this.ConexaoDB, this.Contexto))
            {
                mapeamento.EstruturaConsulta.Take = 1;

                var sqlValorScalar = mapeamento.RetornarSql(false, false, new FiltroMapeamentoVazio());
                return this.ConexaoDB.RetornarValorScalar(sqlValorScalar, mapeamento.Parametros);
            }
        }
    }
}