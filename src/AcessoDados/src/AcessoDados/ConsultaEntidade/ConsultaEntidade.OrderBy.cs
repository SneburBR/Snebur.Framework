using Snebur.AcessoDados.Ajudantes;
using Snebur.Dominio;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Snebur.AcessoDados
{
    public partial class ConsultaEntidade<TEntidade> where TEntidade : IEntidade
    {
        public ConsultaEntidade<TEntidade> OrderBy(Expression<Func<TEntidade, object>> expressaoCaminhoPropriedade)
        {
            return this.Ordernar(this.EstruturaConsulta, expressaoCaminhoPropriedade, EnumSentidoOrdenacao.Crescente);
        }


        public ConsultaEntidade<TEntidade> OrderByDescending(Expression<Func<TEntidade, object>> expressaoCaminhoPropriedade)
        {
            return this.Ordernar(this.EstruturaConsulta, expressaoCaminhoPropriedade, EnumSentidoOrdenacao.Decrescente);
        }

        private ConsultaEntidade<TEntidade> Ordernar(EstruturaConsulta consultaAcessoDados, Expression expressaoCaminhoPropriedade, EnumSentidoOrdenacao sentido)
        {
            var propriedades = ExpressaoUtil.RetornarPropriedades(expressaoCaminhoPropriedade);

            var caminhoPropriedade = AjudanteConsultaEntidade.RetornarCaminhoPropriedade(propriedades);

            var propriedadesCaminhoRelacaoEntidade = new List<PropertyInfo>();
            foreach (var propriedade in propriedades)
            {
                propriedadesCaminhoRelacaoEntidade.Add(propriedade);

                if (AjudanteConsultaEntidade.IsPropriedadeRetornarListaEntidade(propriedade))
                {
                    throw new ErroNaoImplementado(String.Format("Não é suportado propriedade  de coleção '{0}' na ordenação, implementar OrderBy(caminhoColecao, caminhoPropriedade) ", propriedade.Name));
                }
                if (propriedade.PropertyType.IsSubclassOf(typeof(Entidade)))
                {
                    var caminhoPropriedadeAbrirRelacaoEntidade = AjudanteConsultaEntidade.RetornarCaminhoPropriedade(propriedadesCaminhoRelacaoEntidade);
                    this.AbrirRelacaoFiltro(consultaAcessoDados, caminhoPropriedadeAbrirRelacaoEntidade);
                    continue;
                }
            }
            var ultimaPropriedade = propriedades.Last();
            if (!ReflexaoUtil.PropriedadeRetornaTipoPrimario(ultimaPropriedade))
            {
                throw new ErroNaoSuportado(String.Format("A propriedade não suportada para ordenacao {0}", ultimaPropriedade.Name));
            }
            var ordenacao = new Ordenacao
            {
                CaminhoPropriedade = caminhoPropriedade,
                SentidoOrdenacaoEnum = sentido
            };
            if (consultaAcessoDados.Ordenacoes.ContainsKey(caminhoPropriedade))
            {
                throw new Exception($"A propriedade {caminhoPropriedade} já foi ordenada");
            }
            consultaAcessoDados.Ordenacoes.Add(caminhoPropriedade, ordenacao);

            return this;
        }

        //private ConsultaEntidade<TEntidade> OrdernarBackup(Expression<Func<TEntidade, object>> caminho, EnumSentidoOrdenacao sentido)
        //{
        //    var expressao = (Expression)caminho;
        //    var propriedades = ExpressaoUtil.RetornarPropriedades(expressao);

        //    var propriedadesCaminho = new List<PropertyInfo>();
        //    propriedades.Reverse();

        //    ConsultaAcessoDados consultaAcessoDadosAtual = this.ConsultaAcessoDados;

        //    foreach (var propriedade in propriedades)
        //    {

        //        propriedadesCaminho.Add(propriedade);

        //        var caminhoPropriedade = AjudanteConsultaEntidade.RetornarCaminhoPropriedade(propriedadesCaminho);

        //        ErroUtil.ValidarStringVazia(caminhoPropriedade, nameof(caminhoPropriedade));

        //        if (ReflexaoUtil.PropriedadeRetornaTipoPrimario(propriedade))
        //        {

        //            var ordenacao = new Ordenacao();
        //            ordenacao.CaminhoPropriedade = caminhoPropriedade;
        //            ordenacao.SentidoOrdenacaoEnum = sentido;
        //            consultaAcessoDadosAtual.Ordenacoes.Add(ordenacao);

        //        }
        //        else if (propriedade.PropertyType.IsSubclassOf(typeof(BaseEntidade)))
        //        {

        //            if (!consultaAcessoDadosAtual.RelacoesAbertaFiltro.ContainsKey(caminhoPropriedade))
        //            {
        //                this.AbrirRelacaoFiltro(caminhoPropriedade);
        //                //throw ErroUtils.Notificar(new Exception(String.Format("A preciso abrir a relação {0} ander de ordena-la, Caminho completo {0}", caminhoPropriedade, caminho)));
        //            }

        //            var ordenacao = new Ordenacao();
        //            ordenacao.CaminhoPropriedade = caminhoPropriedade;
        //            ordenacao.SentidoOrdenacaoEnum = sentido;
        //            consultaAcessoDadosAtual.Ordenacoes.Add(ordenacao);

        //        }
        //        else if (AjudanteConsultaEntidade.PropriedadeRetornarListaEntidade(propriedade))
        //        {

        //            if (!consultaAcessoDadosAtual.ColecoesAberta.ContainsKey(caminhoPropriedade))
        //            {
        //                throw new Erro(String.Format("A preciso abrir a relação {0} ander de ordena-la, Caminho completo {1}", caminhoPropriedade, caminho));
        //            }

        //            var ordenacao = new Ordenacao();
        //            ordenacao.CaminhoPropriedade = caminhoPropriedade;
        //            ordenacao.SentidoOrdenacaoEnum = sentido;
        //            consultaAcessoDadosAtual.Ordenacoes.Add(ordenacao);

        //            var relacaoAbertaColecao = (RelacaoAbertaColecao)consultaAcessoDadosAtual.ColecoesAberta[caminhoPropriedade];
        //            consultaAcessoDadosAtual = relacaoAbertaColecao.ConsultaAcessoDados;
        //            propriedadesCaminho.Clear();

        //        }
        //        else
        //        {
        //            var mensagemErro = String.Format("O tipo {0} na propriedade {1} não é suportado ", propriedade.PropertyType.Name, propriedade.Name);
        //            throw new Erro(mensagemErro);
        //        }
        //    }
        //    return this;
        //}
    }
}