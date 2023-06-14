using Snebur.Dominio;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Snebur.AcessoDados
{
    public static class ContextoDadosExtensaoAsync
    {
        public static Task SalvarPropriedadesAsync<TEntidade>(this BaseContextoDados contexto,
                                                        TEntidade entidade,
                                                        params Expression<Func<TEntidade, object>>[] expressoesPropriedade) where TEntidade : Entidade
        {
            return Task.Factory.StartNew(() =>
            {
                contexto.SalvarPropriedades(entidade, expressoesPropriedade);
            });
        }

        public static Task RecuperarPropriedadeAsync<TEntidade>(this BaseContextoDados contexto,
                                                                TEntidade entidade, Expression<Func<TEntidade, object>> expressaoPropriedade)
                                                                where TEntidade : Entidade
        {
            return Task.Factory.StartNew(() =>
            {
                contexto.RecuperarPropriedade(entidade, expressaoPropriedade);
            });
        }

        public static Task RecuperarPropriedadeAsync<TEntidade>(this BaseContextoDados contexto, 
                                                                List<TEntidade> entidades,
                                                                Expression<Func<TEntidade, object>> expressaoPropriedade) where TEntidade : Entidade
        {
            return Task.Factory.StartNew(() =>
            {
                contexto.RecuperarPropriedade(entidades, expressaoPropriedade);
            });
        }

        public static Task RecuperarPropriedadesAsync<TEntidade>(this BaseContextoDados contexto, List<TEntidade> entidades, params Expression<Func<TEntidade, object>>[] expressoesPropriedade) where TEntidade : Entidade
        {
            return Task.Factory.StartNew(() =>
            {
                contexto.RecuperarPropriedades(entidades, expressoesPropriedade);
            });
        }

        public static Task AbrirRelacaoAsync<TEntidade, TRelacao>(this BaseContextoDados contexto, TEntidade entidade, Expression<Func<TEntidade, TRelacao>> expressaoRelacao) where TEntidade : Entidade  where TRelacao : Entidade 
        {
            return Task.Factory.StartNew(() =>
            {
                contexto.AbrirRelacao(entidade, expressaoRelacao);
            });
        }

        public static Task AbrirRelacoesAsync<TEntidade, TRelacao>(this BaseContextoDados contexto, 
                                                         TEntidade entidade, 
                                                         params Expression<Func<TEntidade, TRelacao>>[] expressoesRelacao) where TEntidade : Entidade 
                                                                                                                           where TRelacao : Entidade
        {
            return Task.Factory.StartNew(() =>
            {
                contexto.AbrirRelacoes(entidade, expressoesRelacao);
            });
        }

        public static Task<ResultadoSalvar> SalvarAsync(this BaseContextoDados contexto,
                                                        params Entidade[] entidades)
        {
            return Task.Factory.StartNew(() =>
            {
                return contexto.Salvar(entidades);
            });
        }

        public static Task<ResultadoSalvar> SalvarAsync(this BaseContextoDados contexto,
                                                        List<Entidade> entidades)
        {
            return Task.Factory.StartNew(() =>
            {
                return contexto.Salvar(entidades);
            });
        }

        public static Task<ResultadoDeletar> ExcluirAsync(this BaseContextoDados contexto,
                                                         List<Entidade> entidades)
        {
            return Task.Factory.StartNew(() =>
            {
                return contexto.Deletar(entidades);
            });
        }

        public static Task<ResultadoDeletar> ExcluirAsync(this BaseContextoDados contexto,
                                                          List<Entidade> entidades,
                                                          string relacoesEmCascata)
        {
            return Task.Factory.StartNew(() =>
            {
                return contexto.Deletar(entidades, relacoesEmCascata);
            });
        }

        public static Task<ResultadoDeletar> ExcluirAsync(this BaseContextoDados contexto,
                                                         Entidade entidade)
        {
            return Task.Factory.StartNew(() =>
            {
                return contexto.Excluir(entidade);
            });
        }

    }
}
