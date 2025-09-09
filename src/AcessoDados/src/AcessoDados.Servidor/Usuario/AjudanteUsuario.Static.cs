namespace Snebur.AcessoDados;

public partial class CacheSessaoUsuario : IDisposable
{
    private partial class AjudanteSessaoUsuarioInterno
    {
        private static readonly object _bloqueio = new object();
        private static AjudanteSessaoUsuarioInterno? _ajudanteSessaoUsuario;

        internal static AjudanteSessaoUsuarioInterno RetornarAjudanteUsuario(BaseContextoDados contexto)
        {
            if (_ajudanteSessaoUsuario == null)
            {
                lock (_bloqueio)
                {
                    if (_ajudanteSessaoUsuario == null)
                    {
                        _ajudanteSessaoUsuario = new AjudanteSessaoUsuarioInterno(contexto,
                                                                                  contexto.EstruturaBancoDados,
                                                                                  contexto.RetornarUsuariosSistemaInterno());
                    }
                }
            }
            //_ajudanteSessaoUsuario.Contexto = contexto;
            return _ajudanteSessaoUsuario;
        }
    }

    public static void FinalizarSessaoUsuario(Guid identificadorSessaoUsuario)
    {
        var cacheSesssao = GerenciadorCacheSessaoUsuario.Instancia.RetornarCacheSessaoUsuario(identificadorSessaoUsuario, true);
        cacheSesssao?.FinalizarSessaoUsuario();
    }
}