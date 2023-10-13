using System;

namespace Snebur.AcessoDados
{
    public partial class CacheSessaoUsuario : IDisposable
    {
        private partial class AjudanteSessaoUsuarioInterno
        {
            private static object _bloqueio { get; set; } = new object();
            private static AjudanteSessaoUsuarioInterno _ajudanteSessaoUsuario { get; set; }

            internal static AjudanteSessaoUsuarioInterno RetornarAjudanteUsuario(BaseContextoDados contexto)
            {
                if (_ajudanteSessaoUsuario == null)
                {
                    lock (_bloqueio)
                    {
                        if (_ajudanteSessaoUsuario == null)
                        {
                            _ajudanteSessaoUsuario = new AjudanteSessaoUsuarioInterno(contexto, 
                                                                                     contexto.RetornarUsuariosSistemaInterno());
                        }
                    }
                }
                _ajudanteSessaoUsuario.Contexto = contexto;
                return _ajudanteSessaoUsuario;
            }


        }

        public static void FinalizarSessaoUsuario(Guid identificadorSessaoUsuario)
        {
            var cacheSesssao = GerenciadorCacheSessaoUsuario.Instancia.RetornarCacheSessaoUsuario(identificadorSessaoUsuario, true);
            cacheSesssao?.FinalizarSessaoUsuario();
        }
    }
}