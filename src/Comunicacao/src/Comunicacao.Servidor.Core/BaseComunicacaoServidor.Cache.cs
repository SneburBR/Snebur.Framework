using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Snebur.Comunicacao
{
    public abstract partial class BaseComunicacaoServidor
    {
        private static TimeSpan TEMPO_CACHE_PADRAO = TimeSpan.FromMinutes(20);

        private static readonly ConcurrentDictionary<string, ConteudoCache> Caches = new ConcurrentDictionary<string, ConteudoCache>();

#if DEBUG
        private static bool __isManterCacheDebug = !Debugger.IsAttached || true;
#else
        private static bool __isManterCacheDebug = true;
#endif


        private Dictionary<string, TimeSpan> _temposManterOperacao;

        private Dictionary<string, TimeSpan> TemposManterOperacao
        {
            get
            {
                if (this.IsManterCache && this._temposManterOperacao == null)
                {
                    this._temposManterOperacao = new Dictionary<string, TimeSpan>();
                }
                return this._temposManterOperacao;
            }
        }

        protected bool IsManterCache { get; set; }
        protected List<string> OperacoesIgnorarCaches { get; } = new List<string>();

        protected TimeSpan TempoCachePadrao { get; set; } = TEMPO_CACHE_PADRAO;

        private string RetornarChaveCache(Requisicao requisicao)
        {
            var parametros = requisicao.Parametros;
            var chaveParametros = String.Join("--", parametros.Select(x => $"{x.Key}={x.Value.GetHashCode().ToString() ?? "null"}"));
            return $"{this.GetType().Name}--{requisicao.Operacao}.{chaveParametros}";
        }

        private string RetornarConteudoCache(Requisicao requisicao)
        {
            if (this.IsManterCache && __isManterCacheDebug &&
                !this.OperacoesIgnorarCaches.Contains(requisicao.Operacao))
            {
                var chave = this.RetornarChaveCache(requisicao);
                if (Caches.TryGetValue(chave, out var conteudo))
                {
                    var tempoCacheOperado = this.RetornarTempoMantarCache(requisicao.Operacao);

                    if (conteudo.Tempo > tempoCacheOperado)
                    {
                        return null;
                    }
                    return conteudo.Conteudo;
                }
            }
            return null;
        }

        private TimeSpan RetornarTempoMantarCache(string operacao)
        {
            if (this.TemposManterOperacao.ContainsKey(operacao))
            {
                return this.TemposManterOperacao[operacao];
            }
            return this.TempoCachePadrao;
        }

        private void SalvarChace(Requisicao requisicao, string conteudo)
        {
            if (this.IsManterCache)
            {
                var chave = this.RetornarChaveCache(requisicao);

                if (Caches.ContainsKey(chave))
                {
                    Caches.TryRemove(chave, out var conteudoCacheAntigo);
                }
                var conteudoCache = new ConteudoCache(conteudo);
                Caches.TryAdd(chave, conteudoCache);
            }
        }

        protected void LimparConteudosCaches()
        {
            if (this.IsManterCache)
            {
                var prefixo = $"{this.GetType().Name}--";
                var chaves = Caches.Keys;

                foreach (var chave in chaves)
                {
                    if (chave.StartsWith(prefixo))
                    {
                        Caches.TryRemove(chave, out var conteudo);
                    }
                }
            }
        }

        private class ConteudoCache
        {
            public string Conteudo { get; }

            public DateTime DataHora { get; }

            public TimeSpan Tempo => DateTime.UtcNow - this.DataHora;

            public ConteudoCache(string conteudo)
            {
                this.Conteudo = conteudo;
                this.DataHora = DateTime.UtcNow;
            }

        }
    }
}
