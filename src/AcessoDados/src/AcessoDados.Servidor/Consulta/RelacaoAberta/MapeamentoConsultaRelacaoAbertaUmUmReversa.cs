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