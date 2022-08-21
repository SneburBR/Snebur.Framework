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
 