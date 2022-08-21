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
}