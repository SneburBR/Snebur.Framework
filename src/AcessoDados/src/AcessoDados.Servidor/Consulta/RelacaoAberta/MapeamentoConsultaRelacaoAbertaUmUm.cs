using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using Snebur.AcessoDados.Estrutura;
using Snebur.AcessoDados.Dominio;

namespace Snebur.AcessoDados.Mapeamento
{
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
}