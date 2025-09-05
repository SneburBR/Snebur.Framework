using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Snebur.Comunicacao;

public abstract partial class BaseComunicacaoServidor
{
    private static TimeSpan TEMPO_CACHE_PADRAO = TimeSpan.FromMinutes(10);
    private static TimeSpan TEMPO_CACHE_MAXIMO_PADRAO = TimeSpan.FromHours(1);

    private static readonly ConcurrentDictionary<string, ConteudoCache> Caches = new ConcurrentDictionary<string, ConteudoCache>();

    private Dictionary<string, TimeSpan>? TemposManterOperacao
    {
        get
        {
            if (this.IsManterCache && field is null)
            {
                field = new Dictionary<string, TimeSpan>();
            }
            return field;
        }
    }

    protected bool IsManterCache { get; set; }

    protected Dictionary<string, int> OperacoesAtivarCache { get; } = new Dictionary<string, int>();
    protected HashSet<string> OperacoesIgnorarCaches { get; } = new HashSet<string>();

    /// <summary>
    /// Intervalo tempo para atualizar sem fazer o usuário esperar a atualização
    /// </summary>
    protected TimeSpan TempoCachePadrao { get; set; } = DebugUtil.IsAttached ? TimeSpan.FromMinutes(2) : TEMPO_CACHE_PADRAO;

    /// <summary>
    /// Tempo máximo do cache, caso não tenha nenhum requisição nesse intervalo e esse tempo for atingindo o usuário deverá esperar atualização do cache
    /// </summary>
    protected virtual TimeSpan RetornarTempoCacheMaximo(string operacao)
    {
        if (this.OperacoesAtivarCache.TryGetValue(operacao, out var tempo))
        {
            if (tempo > 0)
            {
                return TimeSpan.FromMinutes(tempo);
            }
        }
        return TEMPO_CACHE_MAXIMO_PADRAO;
    }

    protected virtual bool IsCacheAtivado(string operacao)
    {
        if (!this.IsManterCache)
        {
            return false;
        }

        if (this.OperacoesAtivarCache.Count > 0)
        {
            return this.OperacoesAtivarCache.ContainsKey(operacao);
        }

        return !this.OperacoesIgnorarCaches.Contains(operacao);
    }

    internal void InicializarCache()
    {
        var metodosComCacheAtribute = this.GetType().GetMethods()
                                                    .Where(x => x.GetCustomAttribute<CacheAttribute>() != null)
                                                    .Select(x => (x.Name, x.GetCustomAttribute<CacheAttribute>()!));

        var metodosComIgnorarCacheAtribute = this.GetType().GetMethods()
                                                           .Where(x => x.GetCustomAttributes(typeof(IgnorarCacheAttribute), true).Length > 0)
                                                           .Select(x => x.Name);

        foreach (var (metodo, attribute) in metodosComCacheAtribute)
        {
            this.OperacoesAtivarCache.Add(metodo, attribute.ExpirarCacheEmMinutos);
        }

        foreach (var metodo in metodosComIgnorarCacheAtribute)
        {
            this.OperacoesIgnorarCaches.Add(metodo);
        }
        this.IsManterCache = this.OperacoesAtivarCache.Count > 0 || this.OperacoesIgnorarCaches.Count > 0;
    }

    private string RetornarResultadoChamadaSerializadoCache(
        Dictionary<string, object?> parametros,
        string operacao,
        EnumTipoSerializacao tipoSerializacao)
    {

        lock (this.RetornarObjetoBloqueioThreadCache(parametros, operacao, tipoSerializacao))
        {
            var conteudoCache = this.RetornarConteudoCache(parametros, operacao, tipoSerializacao);
            if (conteudoCache != null)
            {
                var tempoCacheOperado = this.RetornarTempoMantarCache(operacao);
                var isAtualzarCache = (DateTime.UtcNow - conteudoCache.DataHora) > tempoCacheOperado;
                if (isAtualzarCache && !conteudoCache.IsAtualizandoCache)
                {
                    conteudoCache.IsAtualizandoCache = true;
                    this._isPodeDispensarServico = false;
                    Task.Run(() => this.AtualizarCache(parametros,
                                                       operacao,
                                                       conteudoCache,
                                                       tipoSerializacao));
                }
                return conteudoCache.Conteudo;
            }

            var resultado = this.RetornarResultadoChamadaSerializadoInterno(parametros, operacao, tipoSerializacao);
            this.SalvarChace(parametros, operacao, resultado, tipoSerializacao);
            return resultado;
        }
    }

    private void AtualizarCache(Dictionary<string, object?> parametros,
                                string operacao,
                                ConteudoCache conteudoCache,
                                EnumTipoSerializacao tipoSerializacao)
    {
        try
        {

            var resultado = this.RetornarResultadoChamadaSerializadoInterno(parametros,
                                                                            operacao,
                                                                            tipoSerializacao);

            this.SalvarChace(parametros, operacao, resultado, tipoSerializacao);
        }
        catch
        {
            Caches.TryRemove(conteudoCache.Chave, out var _);
        }
        finally
        {
            conteudoCache.IsAtualizandoCache = false;
            this._isPodeDispensarServico = true;
            this.Dispose();
        }
    }

    private string RetornarChaveCache(
        Dictionary<string, object?> parametros,
        string operacao,
        EnumTipoSerializacao tipoSerializacao)
    {
        var chaveParametros = String.Join("--", parametros.Select(x => $"{x.Key}={x.Value?.GetHashCode().ToString() ?? "null"}"));
        return $"{this.IdentificadorProprietario}-{this.GetType().Name}--{operacao}.{chaveParametros}--{tipoSerializacao}";
    }
    private ConteudoCache? RetornarConteudoCache(
        Dictionary<string, object?> parametros,
        string operacao,
        EnumTipoSerializacao tipoSerializacao)
    {
        if (this.IsCacheAtivado(operacao))
        {
            var chave = this.RetornarChaveCache(parametros,
                                                operacao,
                                                tipoSerializacao);

            if (Caches.TryGetValue(chave, out var conteudo))
            {
                if (conteudo.Tempo > this.RetornarTempoCacheMaximo(operacao))
                {
                    return null;
                }
                return conteudo;
            }
        }
        return null;
    }

    private TimeSpan RetornarTempoMantarCache(string operacao)
    {
        if (this.TemposManterOperacao?.ContainsKey(operacao) == true)
        {
            return this.TemposManterOperacao[operacao];
        }
        return this.TempoCachePadrao;
    }

    private void SalvarChace(Dictionary<string, object?> parametros,
                            string operacao,
                            string conteudo,
                            EnumTipoSerializacao tipoSerializacao)
    {
        if (this.IsCacheAtivado(operacao))
        {
            var chave = this.RetornarChaveCache(parametros, operacao, tipoSerializacao);
            if (Caches.ContainsKey(chave))
            {
                if (Caches.TryRemove(chave, out var cache))
                {
                    cache.IsAtualizandoCache = false;
                }
            }
            Caches.TryAdd(chave, new ConteudoCache(chave, conteudo));
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
        public string Chave { get; }

        public DateTime DataHora { get; }
        public TimeSpan Tempo => DateTime.UtcNow - this.DataHora;
        public bool IsAtualizandoCache { get; set; }
        public ConteudoCache(string chave,
                             string conteudo)
        {
            this.Chave = chave;
            this.Conteudo = conteudo;
            this.DataHora = DateTime.UtcNow;
        }
    }
}
