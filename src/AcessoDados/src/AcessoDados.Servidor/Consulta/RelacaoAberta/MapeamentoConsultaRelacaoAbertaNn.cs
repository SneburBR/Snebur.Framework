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
    internal class MapeamentoConsultaRelacaoAbertaNn : MapeamentoConsultaRelacaoAberta
    {
        internal EstruturaRelacaoNn EstruturaRelacaoNn
        {
            get
            {
                return (EstruturaRelacaoNn)this.EstruturaRelacao;
            }
        }

        internal MapeamentoConsultaRelacaoAbertaNn(EstruturaConsulta estruturaConsulta, 
                                                   EstruturaBancoDados estruturaBancoDados,
                                                   BaseConexao conexaoDB,
                                                   MapeamentoConsulta mapeamentoConsultaPai,
                                                   EstruturaRelacaoNn estruturaReleacaoNn,
                                                   BaseRelacaoAberta relacaoAberta, 
                                                   BaseContextoDados contexto) :
                                                   base(estruturaConsulta,
                                                        estruturaBancoDados,
                                                        conexaoDB,
                                                        mapeamentoConsultaPai,
                                                        estruturaReleacaoNn,
                                                        relacaoAberta,
                                                        contexto)
        {
        }
    }
}