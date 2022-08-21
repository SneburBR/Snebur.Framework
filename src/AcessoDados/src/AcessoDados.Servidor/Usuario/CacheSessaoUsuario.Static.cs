using System;
using System.Collections;
using System.Collections.Generic;
using Snebur.Dominio;
using Snebur.Seguranca;

namespace Snebur.AcessoDados
{
    public partial class CacheSessaoUsuario
    {
        private const int MAXIMO_TENTATIVA_RETORNAR_SESSAO_USUARIO = 10;
        private static Dictionary<Guid, CacheSessaoUsuario> CachesSessaoUsuario { get; } = new Dictionary<Guid, CacheSessaoUsuario>();
        //private static object Bloqueio = ((ICollection)CachesSessaoUsuario).SyncRoot;
        //private static object Bloqueio = new object();
        private static void RemoverCacheSessaoUsuario(Guid identificadorSessaoUsuario)
        {
            if (CacheSessaoUsuario.CachesSessaoUsuario.ContainsKey(identificadorSessaoUsuario))
            {
                lock (SessaoUsuarioExtensao.RetornarBloqueio(identificadorSessaoUsuario))
                {
                    if (CacheSessaoUsuario.CachesSessaoUsuario.ContainsKey(identificadorSessaoUsuario))
                    {
                        var cache = CacheSessaoUsuario.CachesSessaoUsuario[identificadorSessaoUsuario];
                        lock (((ICollection)CacheSessaoUsuario.CachesSessaoUsuario).SyncRoot)
                        {
                            CacheSessaoUsuario.CachesSessaoUsuario.Remove(identificadorSessaoUsuario);
                        }
                        cache.Dispose();

                    }
                }
            }
        }

        public static CacheSessaoUsuario RetornarCacheSessaoUsuario(BaseContextoDados contexto, Credencial credencial, Guid identificadorSessaoUsuario, InformacaoSessaoUsuario informacaoSessaoUsuario)
        {
            return CacheSessaoUsuario.RetornarCacheSessaoUsuarioInterno(contexto, credencial, identificadorSessaoUsuario, informacaoSessaoUsuario);
        }

        public static CacheSessaoUsuario RetornarCacheSessaoUsuario(Guid identificadorSessaoUsuario, 
                                                                     bool isIgnorarErro = false)
        {
            if (CacheSessaoUsuario.CachesSessaoUsuario.ContainsKey(identificadorSessaoUsuario))
            {
                var cacheSssaoUsuario = CacheSessaoUsuario.CachesSessaoUsuario[identificadorSessaoUsuario];
                cacheSssaoUsuario.NotificarSessaoAtivaAsync();
                return CacheSessaoUsuario.CachesSessaoUsuario[identificadorSessaoUsuario];
            }
            if (isIgnorarErro)
            {
                return null;
            }
            throw new Erro($"O identificador sessão do usuário não foi encontrado{identificadorSessaoUsuario} em {nameof(CacheSessaoUsuario)}");
        }



        private static CacheSessaoUsuario RetornarCacheSessaoUsuarioInterno(BaseContextoDados contexto, Credencial credencial, Guid identificadorSessaoUsuario, InformacaoSessaoUsuario informacaoSessaoUsuario)
        {
            //if (tentativa > 0)
            //{
            //    Thread.Sleep(200 * tentativa);
            //    LogUtil.ErroAsync(new Erro("Nova tentativa de inicializar cache da sessao do usuario"));
            //}

            //if (tentativa > MAXIMO_TENTATIVA_RETORNAR_SESSAO_USUARIO)
            //{
            //    throw new Erro("O maximo de tentativa de retornar o resultado de sessão usuario foi atingido");
            //}

            if (CacheSessaoUsuario.CachesSessaoUsuario.ContainsKey(identificadorSessaoUsuario))
            {
                var cacheSssaoUsuario = CacheSessaoUsuario.CachesSessaoUsuario[identificadorSessaoUsuario];

                if (cacheSssaoUsuario.IsInicializado &&
                   cacheSssaoUsuario.Usuario != null ||
                   cacheSssaoUsuario.SessaoUsuario != null)
                {
                    cacheSssaoUsuario.NotificarSessaoAtivaAsync();
                    return CacheSessaoUsuario.CachesSessaoUsuario[identificadorSessaoUsuario];
                }
                CacheSessaoUsuario.RemoverCacheSessaoUsuario(identificadorSessaoUsuario);
            }

            var bloqueio = SessaoUsuarioExtensao.RetornarBloqueio(identificadorSessaoUsuario);
            lock (bloqueio)
            {
                if (!CacheSessaoUsuario.CachesSessaoUsuario.ContainsKey(identificadorSessaoUsuario))
                {
                    var novoCacheSessaoUsuario = new CacheSessaoUsuario(contexto, credencial, identificadorSessaoUsuario, informacaoSessaoUsuario);
                    novoCacheSessaoUsuario.Inicializar();


                    lock (((ICollection)CacheSessaoUsuario.CachesSessaoUsuario).SyncRoot)
                    {
                        CacheSessaoUsuario.CachesSessaoUsuario.Add(identificadorSessaoUsuario, novoCacheSessaoUsuario);
                    }

                    return novoCacheSessaoUsuario;
                }
                else
                {
                    return RetornarCacheSessaoUsuarioInterno(contexto, credencial, identificadorSessaoUsuario, informacaoSessaoUsuario);
                }
            }
        }
    }
}

