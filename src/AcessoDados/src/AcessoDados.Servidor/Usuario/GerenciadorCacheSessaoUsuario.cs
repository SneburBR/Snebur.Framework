using Snebur.Dominio;
using Snebur.Seguranca;
using Snebur.Utilidade;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Snebur.AcessoDados
{
    public class GerenciadorCacheSessaoUsuario
    {

        private const int MAXIMO_TENTATIVA_RETORNAR_SESSAO_USUARIO = 10;
        //private Dictionary<Guid, CacheSessaoUsuario> CachesSessaoUsuario { get; } = new Dictionary<Guid, CacheSessaoUsuario>();
        private ConcurrentDictionary<Guid, CacheSessaoUsuario> CachesSessaoUsuario { get; } = new ConcurrentDictionary<Guid, CacheSessaoUsuario>();
        private GerenciadorCacheSessaoUsuario()
        {

        }

        public static GerenciadorCacheSessaoUsuario Instancia { get; } = new GerenciadorCacheSessaoUsuario();

        public void RemoverCacheSessaoUsuario(Guid identificadorSessaoUsuario)
        {
            lock (SessaoUsuarioExtensao.RetornarBloqueio(identificadorSessaoUsuario))
            {
                if (this.CachesSessaoUsuario.TryGetValue(identificadorSessaoUsuario, out var cache))
                {
                    this.CachesSessaoUsuario.TryRemove(identificadorSessaoUsuario, out var xxx);
                    cache.Dispose();
                }
            }
        }

        public CacheSessaoUsuario RetornarCacheSessaoUsuario(BaseContextoDados contexto,
                                                            Credencial credencial,
                                                            Guid identificadorSessaoUsuario,
                                                            InformacaoSessaoUsuario informacaoSessaoUsuario)
        {
            return this.RetornarCacheSessaoUsuarioInterno(contexto, credencial, identificadorSessaoUsuario, informacaoSessaoUsuario);
        }

        public CacheSessaoUsuario RetornarCacheSessaoUsuario(Guid identificadorSessaoUsuario,
                                                             bool isIgnorarErro = false)
        {
            lock (SessaoUsuarioExtensao.RetornarBloqueio(identificadorSessaoUsuario))
            {
                if (this.CachesSessaoUsuario.TryGetValue(identificadorSessaoUsuario, out var cacheSssaoUsuario))
                {
                    cacheSssaoUsuario.NotificarSessaoAtivaAsync();
                    return cacheSssaoUsuario;
                }
            }
           

            //return this.CachesSessaoUsuario[identificadorSessaoUsuario];

            if (isIgnorarErro)
            {
                return null;
            }
            throw new Erro($"O identificador sessão do usuário não foi encontrado{identificadorSessaoUsuario} em {nameof(CacheSessaoUsuario)}");
        }



        private CacheSessaoUsuario RetornarCacheSessaoUsuarioInterno(BaseContextoDados contexto,
                                                                      Credencial credencial,
                                                                      Guid identificadorSessaoUsuario,
                                                                      InformacaoSessaoUsuario informacaoSessaoUsuario)
        {
            lock (SessaoUsuarioExtensao.RetornarBloqueio(identificadorSessaoUsuario))
            {
                if (this.CachesSessaoUsuario.TryGetValue(identificadorSessaoUsuario,
                                                         out var cacheSssaoUsuario))
                {
                    if (cacheSssaoUsuario.IsInicializado &&
                        cacheSssaoUsuario.Usuario != null ||
                        cacheSssaoUsuario.SessaoUsuario != null)
                    {
                        cacheSssaoUsuario.Contexto = contexto;
                        cacheSssaoUsuario.NotificarSessaoAtivaAsync();
                        return cacheSssaoUsuario;
                    }
                    this.RemoverCacheSessaoUsuario(identificadorSessaoUsuario);
                }

                var novoCacheSessaoUsuario = new CacheSessaoUsuario(contexto,
                                                                   credencial,
                                                                   identificadorSessaoUsuario,
                                                                   informacaoSessaoUsuario);
                novoCacheSessaoUsuario.Inicializar();
                if (this.CachesSessaoUsuario.TryAdd(identificadorSessaoUsuario, novoCacheSessaoUsuario))
                {
                    return novoCacheSessaoUsuario;
                }
                return this.RetornarCacheSessaoUsuarioInterno(contexto,
                                                              credencial,
                                                              identificadorSessaoUsuario,
                                                              informacaoSessaoUsuario);

            }
        }
    }
}

