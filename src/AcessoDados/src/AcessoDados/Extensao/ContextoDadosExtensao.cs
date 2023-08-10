using Snebur.Dominio;
using Snebur.Utilidade;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Snebur.AcessoDados
{
    public static class ContextoDadosExtensao
    {
        public static void SalvarPropriedade<TEntidade>(this IContextoDados contexto,
                                                         TEntidade entidade,
                                                         Expression<Func<TEntidade, object>> expressaoPropriedad) where TEntidade : Entidade
        {
            SalvarPropriedades(contexto, entidade, expressaoPropriedad);
        }
        public static void SalvarPropriedade<TEntidade>(this IContextoDados contexto,
                                                        IEnumerable<TEntidade> entidades,
                                                        Expression<Func<TEntidade, object>> expressaoPropriedad) where TEntidade : Entidade
        {
            SalvarPropriedades(contexto, entidades, expressaoPropriedad);
        }


        public static void SalvarPropriedades<TEntidade>(this IContextoDados contexto,
                                                          TEntidade entidade,
                                                          params Expression<Func<TEntidade, object>>[] expressoesPropriedade) where TEntidade : Entidade
        {

            var clone = entidade.CloneSomenteId(expressoesPropriedade);
            var propriedadesAbertas = new List<string>();
            foreach (var expressao in expressoesPropriedade)
            {
                var propriedade = ExpressaoUtil.RetornarPropriedade(expressao);
                if (propriedade.TrySetValue(clone, propriedade.GetValue(entidade)))
                {
                    propriedadesAbertas.Add(propriedade.Name);
                }
            }
            var entidadeInterna = (IEntidadeInterna)clone;
            entidadeInterna.AtribuirPropriedadesAbertas(propriedadesAbertas);
            contexto.Salvar(clone);
        }

        public static void SalvarPropriedades<TEntidade>(this IContextoDados contexto,
                                                         IEnumerable<TEntidade> entidades,
                                                         params Expression<Func<TEntidade, object>>[] expressoesPropriedade) where TEntidade : Entidade
        {

            var entidadesSalvar = new List<TEntidade>();
            foreach(var entidade in entidades)
            {
                var clone = entidade.CloneSomenteId(expressoesPropriedade);
                var propriedadesAbertas = new List<string>();
                foreach (var expressao in expressoesPropriedade)
                {
                    var propriedade = ExpressaoUtil.RetornarPropriedade(expressao);
                    if (propriedade.TrySetValue(clone, propriedade.GetValue(entidade)))
                    {
                        propriedadesAbertas.Add(propriedade.Name);
                    }
                }
                var entidadeInterna = (IEntidadeInterna)clone;
                entidadeInterna.AtribuirPropriedadesAbertas(propriedadesAbertas);
                entidadesSalvar.Add(clone);
            }
            contexto.Salvar(entidadesSalvar);
        }



        public static void RecuperarPropriedade<TEntidade>(this IContextoDados contexto,
                                                          TEntidade entidade,
                                                          Expression<Func<TEntidade, object>> expressaoPropriedade) where TEntidade : Entidade
        {
            RecuperarPropriedades(contexto,
                                  new List<TEntidade> { entidade },
                                  expressaoPropriedade);
        }

        public static void RecuperarPropriedade<TEntidade>(this IContextoDados contexto,
                                                           List<TEntidade> entidades,
                                                           Expression<Func<TEntidade, object>> expressaoPropriedade) where TEntidade : Entidade
        {
            RecuperarPropriedades(contexto,
                                  entidades,
                                  expressaoPropriedade);
        }



        public static void RecuperarPropriedades<TEntidade>(this IContextoDados contexto,
                                                            TEntidade entidade,
                                                            params Expression<Func<TEntidade, object>>[] expressoesPropriedade) where TEntidade : Entidade
        {
            RecuperarPropriedades(contexto,
                                  new List<TEntidade> { entidade },
                                  expressoesPropriedade);
        }
        public static void RecuperarPropriedades<TEntidade>(this IContextoDados contexto,
                                                        List<TEntidade> entidades,
                                                        params Expression<Func<TEntidade, object>>[] expressoesPropriedade) where TEntidade : Entidade
        {
            var ids = entidades.Select(x => x.Id).ToList();
            var entidadesRecuperada = contexto.RetornarConsulta<TEntidade>(typeof(TEntidade)).
                                              WhereIds(ids).
                                              AbrirPropriedades(expressoesPropriedade).ToList();

            foreach (var expressao in expressoesPropriedade)
            {
                var propriedade = ExpressaoUtil.RetornarPropriedade(expressao);
                foreach (var entidade in entidades)
                {
                    var entidadeRecuperada = entidadesRecuperada.Where(x => x.Id == entidade.Id).Single();
                    propriedade.SetValue(entidade, propriedade.GetValue(entidadeRecuperada));
                }

            }
        }

        public static void AbrirRelacao<TEntidade, TRelacao>(this IContextoDados contexto,
                                                    TEntidade entidade, Expression<Func<TEntidade, TRelacao>> expressaoRelacao) where TEntidade : Entidade where TRelacao : Entidade
        {
            AbrirRelacoes(contexto, entidade, expressaoRelacao);
        }
        public static void AbrirRelacao<TEntidade, TRelacao>(this IContextoDados contexto,
                                                    IEnumerable<TEntidade> entidade, Expression<Func<TEntidade, TRelacao>> expressaoRelacao) where TEntidade : Entidade where TRelacao : Entidade
        {
            AbrirRelacoes(contexto, entidade, expressaoRelacao);
        }

        public static void AbrirRelacoes<TEntidade, TRelacao>(this IContextoDados contexto,
                                                    TEntidade entidade,
                                                    params Expression<Func<TEntidade, TRelacao>>[] expressoesRelacao) where TEntidade : Entidade where TRelacao : Entidade
        {

            var consulta = contexto.RetornarConsulta<TEntidade>(entidade.GetType()).
                                              Where(x => x.Id == entidade.Id);
            consulta.AbrirRelacoes(expressoesRelacao);
            var entidadeRecuperada = consulta.Single();

            foreach (var expressao in expressoesRelacao)
            {
                var propriedades = ExpressaoUtil.RetornarPropriedades(expressao);
                var propriedade = propriedades.First();
                propriedade.SetValue(entidade, propriedade.GetValue(entidadeRecuperada));
            }
        }

        public static void AbrirRelacoes<TEntidade, TRelacao>(this IContextoDados contexto,
                                            IEnumerable<TEntidade> entidades,
                                            params Expression<Func<TEntidade, TRelacao>>[] expressoesRelacao) where TEntidade : Entidade where TRelacao : Entidade
        {

            var ids = entidades.Select(x => x.Id).ToList();
            var consulta = contexto.RetornarConsulta<TEntidade>(typeof(TEntidade)).
                                                    WhereIds(ids);
            consulta.AbrirRelacoes(expressoesRelacao);
            var entidadesRecuperada = consulta.ToList();
            foreach (var entidade in entidades)
            {
                var entidadeRecuperada = entidadesRecuperada.Where(x => x.Id == entidade.Id).Single();
                foreach (var expressao in expressoesRelacao)
                {
                    var propriedades = ExpressaoUtil.RetornarPropriedades(expressao);
                    var propriedade = propriedades.First();
                    propriedade.SetValue(entidade, propriedade.GetValue(entidadeRecuperada));
                }
            }
        }

        public static void AbrirColecoes<TEntidade>(this IContextoDados contexto,
                                                    TEntidade entidade,
                                                    params Expression<Func<TEntidade, IEnumerable>>[] expressoesColecao) where TEntidade : Entidade
        {
            var entidadeRecuperada = contexto.RetornarConsulta<TEntidade>(entidade.GetType()).
                                              Where(x => x.Id == entidade.Id).
                                              AbrirColecoes(expressoesColecao).Single();

            foreach (var expressao in expressoesColecao)
            {
                var propriedade = ExpressaoUtil.RetornarPropriedade(expressao);
                propriedade.SetValue(entidade, propriedade.GetValue(entidadeRecuperada));
            }
        }
    }
}