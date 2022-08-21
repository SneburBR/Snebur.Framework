using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Snebur.Dominio;
using Snebur.Utilidade;

namespace Snebur.AcessoDados
{
    public static class ContextoDadosExtensao
    {
        public static void SalvarPropriedade<TEntidade>(this ContextoDados contexto,
                                                       TEntidade entidade,
                                                        Expression<Func<TEntidade, object>> expressaoPropriedad) where TEntidade : Entidade
        {
            SalvarPropriedades(contexto, entidade, expressaoPropriedad);
        }


        public static void SalvarPropriedades<TEntidade>(this ContextoDados contexto,
                                                          TEntidade entidade,
                                                          params Expression<Func<TEntidade, object>>[] expressoesPropriedade) where TEntidade : Entidade
        {

            var clone = entidade.CloneSomenteId<Entidade>();
            var propriedadesAbertas = new HashSet<string>();
            foreach (var expressao in expressoesPropriedade)
            {
                var propriedade = ExpressaoUtil.RetornarPropriedade(expressao);
                if(propriedade.CanRead && propriedade.CanWrite)
                {
                    propriedade.SetValue(clone, propriedade.GetValue(entidade));
                }
                
                propriedadesAbertas.Add(propriedade.Name);
            }
            var entidadeInterna = (IEntidadeInterna)clone;
            entidadeInterna.AtribuirPropriedadesAbertas(propriedadesAbertas);
            contexto.Salvar(clone);
        }



        public static void RecuperarPropriedade<TEntidade>(this ContextoDados contexto,
                                                          TEntidade entidade,
                                                          Expression<Func<TEntidade, object>> expressaoPropriedade) where TEntidade : Entidade
        {
            RecuperarPropriedades(contexto,
                                  new List<TEntidade> { entidade },
                                  expressaoPropriedade);
        }

        public static void RecuperarPropriedade<TEntidade>(this ContextoDados contexto,
                                                           List<TEntidade> entidades,
                                                           Expression<Func<TEntidade, object>> expressaoPropriedade) where TEntidade : Entidade
        {
            RecuperarPropriedades(contexto,
                                  entidades,
                                  expressaoPropriedade);
        }



        public static void RecuperarPropriedades<TEntidade>(this ContextoDados contexto,
                                                            TEntidade entidade,
                                                            params Expression<Func<TEntidade, object>>[] expressoesPropriedade) where TEntidade : Entidade
        {
            RecuperarPropriedades(contexto,
                                  new List<TEntidade> { entidade },
                                  expressoesPropriedade);
        }
        public static void RecuperarPropriedades<TEntidade>(this ContextoDados contexto,
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

        public static void AbrirRelacao<TEntidade>(this ContextoDados contexto, TEntidade entidade, Expression<Func<TEntidade, object>> expressaoRelacao) where TEntidade : Entidade
        {
            AbrirRelacoes(contexto, entidade, expressaoRelacao);
        }

        public static void AbrirRelacoes<TEntidade>(this ContextoDados contexto,
                                                    TEntidade entidade,
                                                    params Expression<Func<TEntidade, object>>[] expressoesRelacao) where TEntidade : Entidade
        {

            var entidadeRecuperada = contexto.RetornarConsulta<TEntidade>(entidade.GetType()).
                                              Where(x => x.Id == entidade.Id).
                                              AbrirRelacoes(expressoesRelacao).Single();

            foreach (var expressao in expressoesRelacao)
            {
                var propriedade = ExpressaoUtil.RetornarPropriedade(expressao);
                propriedade.SetValue(entidade, propriedade.GetValue(entidadeRecuperada));
            }
        }


        //public static void AbrirColecao<TEntidade, TEntidadeRelacao>(this IContextoSigiX contexto, 
        //                                          TEntidade entidade, Expression<Func<TEntidade, 
        //                                          IEnumerable<TEntidadeRelacao>>> expressaoColecao) where TEntidade : Entidade
        //{
        //    AbrirColecoes(contexto, entidade, expressaoColecao);
        //}

        public static void AbrirColecoes<TEntidade>(this ContextoDados contexto, TEntidade entidade, params Expression<Func<TEntidade, IEnumerable>>[] expressoesColecao) where TEntidade : Entidade
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

        //public static void AbrirColecoes<TEntidade>(this IContextoSigiX contexto, TEntidade entidade, params Expression<Func<TEntidade, IEnumerable<IListaEntidades>>>[] expressoesColecao) where TEntidade : Entidade
        //{
        //    var caminhosPropriedade = ExpressaoUtil.RetornarPropriedades()

        //}


        //public static void RecuperarPropriedades<TEntidade>(this IContextoSigiX contexto, TEntidade entidade, params Expression<Func<TEntidade, object>>[] expressoesPropriedade) where TEntidade : Entidade
        //{

        //    var entidadeRecuperada = contexto.RetornarConsulta<TEntidade>(entidade.GetType()).Where(x => x.Id == entidade.Id).
        //        AbrirPropriedades(expressoesPropriedade).Single();

        //    foreach (var expressao in expressoesPropriedade)
        //    {
        //        var propriedade = ExpressaoUtil.RetornarPropriedade(expressao);
        //        propriedade.SetValue(entidade, propriedade.GetValue(entidadeRecuperada));
        //    }
        //}

    }
}