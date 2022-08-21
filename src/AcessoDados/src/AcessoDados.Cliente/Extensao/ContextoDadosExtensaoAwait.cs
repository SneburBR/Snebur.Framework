using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Snebur.Dominio;

namespace Snebur.AcessoDados
{
    public static class ContextoDadosExtensaoAwait
    {
        public static Task SalvarPropriedadesAwait<TEntidade>(this ContextoDados contexto,
                                                        TEntidade entidade,
                                                        params Expression<Func<TEntidade, object>>[] expressoesPropriedade) where TEntidade : Entidade
        {
            return Task.Factory.StartNew(() =>
            {
                contexto.SalvarPropriedades(entidade, expressoesPropriedade);
            });
        }

        public static Task RecuperarPropriedadeAwait<TEntidade>(this ContextoDados contexto, 
                                                                TEntidade entidade, Expression<Func<TEntidade, object>> expressaoPropriedade)
                                                                where TEntidade : Entidade
        {
            return Task.Factory.StartNew(() =>
            {
                contexto.RecuperarPropriedade(entidade, expressaoPropriedade);
            });
        }

        public static Task RecuperarPropriedadeAwait<TEntidade>(this ContextoDados contexto, List<TEntidade> entidades,
                                                                Expression<Func<TEntidade, object>> expressaoPropriedade) where TEntidade : Entidade
        {
            return Task.Factory.StartNew(() =>
            {
                contexto.RecuperarPropriedade(entidades, expressaoPropriedade);
            });
        }

        public static Task RecuperarPropriedadesAwait<TEntidade>(this ContextoDados contexto, List<TEntidade> entidades, params Expression<Func<TEntidade, object>>[] expressoesPropriedade) where TEntidade : Entidade
        {
            return Task.Factory.StartNew(() =>
            {
                contexto.RecuperarPropriedades(entidades, expressoesPropriedade);
            });
        }

        public static Task AbrirRelacaoAwait<TEntidade>(this ContextoDados contexto, TEntidade entidade, Expression<Func<TEntidade, object>> expressaoRelacao) where TEntidade : Entidade
        {
            return Task.Factory.StartNew(() =>
            {
                contexto.AbrirRelacao(entidade, expressaoRelacao);
            });
        }

        public static Task AbrirRelacoesAwait<TEntidade>(this ContextoDados contexto, TEntidade entidade, params Expression<Func<TEntidade, object>>[] expressoesRelacao) where TEntidade : Entidade
        {
            return Task.Factory.StartNew(() =>
            {
                contexto.AbrirRelacoes(entidade, expressoesRelacao);
            });
        }

        public static Task<ResultadoSalvar> SalvarAwait(this ContextoDados contexto,
                                                        params Entidade[] entidades)
        {
            return Task.Factory.StartNew(() =>
            {
                return contexto.Salvar(entidades);
            });
        }

        public static Task<ResultadoSalvar> SalvarAwait(this ContextoDados contexto,
                                                        List<Entidade> entidades)
        {
            return Task.Factory.StartNew(() =>
            {
                return contexto.Salvar(entidades);
            });
        }

        public static Task<ResultadoExcluir> ExcluirAwait(this ContextoDados contexto,
                                                     List<Entidade> entidades)
        {
            return Task.Factory.StartNew(() =>
            {
                return contexto.Excluir(entidades);
            });
        }

        public static Task<ResultadoExcluir> ExcluirAwait(this ContextoDados contexto,
                                                          List<Entidade> entidades,
                                                          string relacoesEmCascata)
        {
            return Task.Factory.StartNew(() =>
            {
                return contexto.Excluir(entidades, relacoesEmCascata);
            });
        }

        public static Task<ResultadoExcluir> ExcluirAwait(this ContextoDados contexto,
                                                         Entidade entidade)
        {
            return Task.Factory.StartNew(() =>
            {
                return contexto.Excluir(entidade);
            });
        }

    }
}
