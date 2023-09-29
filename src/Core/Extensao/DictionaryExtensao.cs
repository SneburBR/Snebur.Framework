using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace System
{
    public static class DictionaryExtensao
    {
        private static readonly Dictionary<int, Dictionary<int, object>> _dicionariosBloqueio = new Dictionary<int, Dictionary<int, object>>();

        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dicionario, TKey key)
        {
            if (dicionario.TryGetValue(key, out TValue valor))
            {
                return valor;
            }
            return default;
        }

        public static void RemoveAll<TKey, TValue>(this Dictionary<TKey, TValue> dicionario, IEnumerable<TKey> keys)
        {
            foreach (var key in keys)
            {
                dicionario.TryRemove(key);
            }
        }

        public static void TryRemove<TKey, TValue>(this Dictionary<TKey, TValue> dicionario, TKey key)
        {
            lock ((dicionario as ICollection).SyncRoot)
            {
                if (dicionario.ContainsKey(key))
                {
                    dicionario.Remove(key);
                }
            }
        }

        public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dicionario,
                                   TKey key,
                                   TValue value)
        {
            lock ((dicionario as ICollection).SyncRoot)
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
        }

        public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dicionario,
                                               TKey key,
                                               TValue value)
        {
            lock ((dicionario as ICollection).SyncRoot)
            {
                if (dicionario.ContainsKey(key))
                {
                    return false;
                }
                dicionario.Add(key, value);
                return true;
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
         
        //private static readonly object _bloqueioDicionario = new object();
        //private static readonly object _bloqueioChave = new object();
        private static object RetornarBloqueio<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dicionario,
                                                            TKey chave)
        {
            var hashDicionario = dicionario.GetHashCode();
            if (!_dicionariosBloqueio.ContainsKey(hashDicionario))
            {
                lock ((_dicionariosBloqueio as ICollection).SyncRoot)
                {
                    if (!_dicionariosBloqueio.ContainsKey(hashDicionario))
                    {
                        _dicionariosBloqueio.TryAdd(hashDicionario, new Dictionary<int, object>());
                    }
                }
            }

            var hashChave = chave.GetHashCode();
            var dicionarioBloqueio = _dicionariosBloqueio[hashDicionario];

            if (!dicionarioBloqueio.ContainsKey(hashChave))
            {
                lock ((_dicionariosBloqueio as ICollection).SyncRoot)
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