using Snebur.Dominio;
using System;
using System.Collections.Generic;

namespace Snebur.AcessoDados.Seguranca
{
    internal partial class SeguracaContextoDados
    {

        private void AplicarRestricoesFiltro(List<AutorizacaoEntidade> autorizacoes, IEnumerable<IEntidade> entidades)
        {
            foreach (var autorizacao in autorizacoes)
            {
                this.AplicarRestricoesFiltro(autorizacao, entidades);
            }
        }

        private void AplicarRestricoesFiltro(AutorizacaoEntidade autorizacao, IEnumerable<IEntidade> entidades)
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