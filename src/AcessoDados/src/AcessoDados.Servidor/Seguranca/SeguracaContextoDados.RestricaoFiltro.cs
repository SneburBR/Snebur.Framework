using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur.Dominio;

namespace Snebur.AcessoDados.Seguranca
{
    internal partial class SeguracaContextoDados
    {

        private void AplicarRestricoesFiltro(List<AutorizacaoEntidade> autorizacoes, List<Entidade> entidades)
        {
            foreach (var autorizacao in autorizacoes)
            {
                this.AplicarRestricoesFiltro(autorizacao, entidades);
            }
        }

        private void AplicarRestricoesFiltro(AutorizacaoEntidade autorizacao, List<Entidade> entidades)
        {
            var restricoes = autorizacao.RetornarRestricoesFiltro();
            if (restricoes.Count > 0)
            {
                throw new ErroNaoImplementado();
            }
        }

        private void AplicarRestricoesFiltro(List<AutorizacaoEntidade> autorizacoes, EstruturaConsulta estruturaConsulta)
        {
            foreach (var autorizacao in autorizacoes)
            {
                this.AplicarRestricoesFiltro(autorizacao, estruturaConsulta);
            }
        }

        private void AplicarRestricoesFiltro(AutorizacaoEntidade autorizacao, EstruturaConsulta estruturaConsulta)
        {
            var restricoes = autorizacao.RetornarRestricoesFiltro();
            if (restricoes.Count > 0)
            {
                throw new ErroNaoImplementado();
            }
        }
    }
}