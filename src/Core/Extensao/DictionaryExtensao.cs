using System.Collections.Concurrent;
using System.Collections.Generic;

namespace System
{
    public static class DictionaryExtensao
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dicionario, TKey key)
        {
            if (dicionario.TryGetValue(key, out TValue valor))
            {
                return valor;
            }
            return default;
        }

        public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dicionario, 
                                       TKey key,
                                       TValue value)
        {
            if (dicionario.ContainsKey(key))
            {
                dicionario[key] = value;
            }
            else
            {
                dicionario.Add(key, value);
            }
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dicionario, TKey key)
        {
            if (dicionario.TryGetValue(key, out TValue valor))
            {
                return valor;
            }
            return default;
        }
        public static TValue GetOrAddWithLockKey<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dicionario,
                                                             TKey chave,
                                                             Func<TKey, TValue> funcaoRetornarValor)
        {
            return GetOrAddWithLockKeyInterno(dicionario,
                                              chave,
                                              null,
                                              funcaoRetornarValor);
        }

        public static TValue GetOrAddWithLockKey<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dicionario,
                                                          TKey chave,
                                                          object userState,
                                                          Func<TKey, object, TValue> funcaoRetornarValor)
        {
            return GetOrAddWithLockKeyInterno(dicionario,
                                              chave,
                                              userState,
                                              funcaoRetornarValor);
        }
        private static TValue GetOrAddWithLockKeyInterno<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dicionario,
                                                         TKey chave,
                                                         object userState,
                                                         Delegate funcaoRetornarValor,
                                                         int tentativa = 0)
        {
            if (chave == null)
            {
                throw new ArgumentNullException(nameof(chave));
            }
            if (funcaoRetornarValor == null)
            {
                throw new ArgumentNullException(nameof(funcaoRetornarValor));
            }
            if (!dicionario.ContainsKey(chave))
            {
                var bloqueio = dicionario.RetornarBloqueio(chave);
                lock (bloqueio)
                {
                    if (!dicionario.ContainsKey(chave))
                    {
                        if (!dicionario.ContainsKey(chave))
                        {
                            TValue novoValor;
                            if (funcaoRetornarValor is Func<TKey, TValue> normal)
                            {
                                novoValor = normal.Invoke(chave);
                            }
                            else if (funcaoRetornarValor is Func<TKey, object, TValue> aaaa)
                            {
                                novoValor = aaaa.Invoke(chave, userState);
                            }
                            else
                            {
                                throw new Erro("O delegate não é suportado");
                            }
                            if (dicionario.TryAdd(chave, novoValor))
                            {
                                return novoValor;
                            }
                        }
                    }
                }
            }
            if (dicionario.TryGetValue(chave, out var valor))
            {
                return valor;
            }
            var erro = new Erro($"A falha a retornar o valor da chave " + chave?.ToString());

#if DEBUG
            throw erro;
#else
            if (tentativa > 5)
            {
                throw erro;
            }
            return GetOrAddWithLockKeyInterno(dicionario, chave, userState, funcaoRetornarValor, tentativa += 1);
#endif
        }
        private static readonly Dictionary<int, Dictionary<int, object>> _dicionariosBloqueio = new Dictionary<int, Dictionary<int, object>>();
        private static readonly object _bloqueioDicionario = new object();
        private static readonly object _bloqueioChave = new object();
        private static object RetornarBloqueio<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dicionario,
                                                            TKey chave)
        {
            var hashDicionario = dicionario.GetHashCode();
            if (!_dicionariosBloqueio.ContainsKey(hashDicionario))
            {
                lock (_bloqueioDicionario)
                {
                    if (!_dicionariosBloqueio.ContainsKey(hashDicionario))
                    {
                        _dicionariosBloqueio.Add(hashDicionario, new Dictionary<int, object>());
                    }
                }
            }
            var hashChave = chave.GetHashCode();
            var dicionarioBloqueio = _dicionariosBloqueio[hashDicionario];

            if (!dicionarioBloqueio.ContainsKey(hashChave))
            {
                lock (_bloqueioDicionario)
                {
                    if (!dicionarioBloqueio.ContainsKey(hashChave))
                    {
                        dicionarioBloqueio.Add(hashChave, new object());
                    }
                }
            }
            return dicionarioBloqueio[hashChave];
        }
    }
}