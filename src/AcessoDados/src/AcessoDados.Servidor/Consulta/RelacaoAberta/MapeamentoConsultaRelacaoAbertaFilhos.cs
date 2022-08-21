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
    internal class MapeamentoConsultaRelacaoAbertaFilhos : MapeamentoConsultaRelacaoAberta
    {
        internal EstruturaRelacaoFilhos EstruturaRelacaoFilhos
        {
            get
            {
                return (EstruturaRelacaoFilhos)this.EstruturaRelacao;
            }
        }
 
        internal MapeamentoConsultaRelacaoAbertaFilhos(EstruturaConsulta estruturaConsulta, 
                                                       EstruturaBancoDados estruturaBancoDados,
                                                       BaseConexao conexaoDB,
                                                       MapeamentoConsulta mapeamentoConsultaPai,
                                                       EstruturaRelacaoFilhos estruturaReleacaoFilhos,
                                                       BaseRelacaoAberta relacaoAberta,
                                                       BaseContextoDados contexto) :
                                                       base(estruturaConsulta,
                                                            estruturaBancoDados,
                                                            conexaoDB,
                                                            mapeamentoConsultaPai,
                                                            estruturaReleacaoFilhos,
                                                            relacaoAberta,
                                                            contexto)
        {
          
        }
    }
}