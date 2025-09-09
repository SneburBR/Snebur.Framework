using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;
namespace Snebur.AcessoDados.Estrutura;

public class DicionarioEstrutura<T> : IEnumerable<KeyValuePair<string, T>>
{
    private readonly Dictionary<string, T> _maps = new(StringComparer.OrdinalIgnoreCase);

    internal T this[string chave]
    {
        get
        {
            return this._maps[chave] ??
                      throw new ErroOperacaoInvalida($"Não existe a estrutura da entidade '{chave}' no dicionario de mapeamento");
        }
    }

    public int Count
        => this._maps.Count;

    internal bool ContainsKey(string chave)
    {
        return this._maps.ContainsKey(chave);
    }
    public IEnumerable<T> Values
        => this._maps.Values;

    public override string ToString()
    {
        return String.Format("Dicionario {0} - {1}", typeof(T), base.ToString());
    }

    internal void Add(string name, T value)
    {
        if (this._maps.ContainsKey(name))
        {
            throw new ErroOperacaoInvalida(
                $"Já existe a estrutura  '{name}' no dicionario de mapeamento");
        }
        this._maps.Add(name, value);
    }

    internal bool TryGetValue(string chave,
        [NotNullWhen(true)]
        [MaybeNullWhen(false)]
        out T? estruturaCampo)
    {
#pragma warning disable CS8762 // Parameter must have a non-null value when exiting in some condition.
        return this._maps.TryGetValue(chave, out estruturaCampo);
#pragma warning restore CS8762 // Parameter must have a non-null value when exiting in some condition.
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this._maps.GetEnumerator();

    }

    public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
    {
        return this._maps.GetEnumerator();
    }

    internal void Clear()
    {
        this._maps.Clear();
    }
}
