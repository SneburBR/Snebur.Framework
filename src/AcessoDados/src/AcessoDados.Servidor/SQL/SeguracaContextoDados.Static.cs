namespace Snebur.AcessoDados.Seguranca
{
    internal partial class SeguracaContextoDados
    {
        private static SeguracaContextoDados _seguracaContextoDados;

        internal static object Bloquerio = new object();

        public static SeguracaContextoDados RetornarSegurancaoContextoDados(IContextoDadosSeguranca contexto)
        {
            if (_seguracaContextoDados == null)
            {
                lock (Bloquerio)
                {
                    if (_seguracaContextoDados == null)
                    {
                        _seguracaContextoDados = new SeguracaContextoDados(contexto);
                    }
                }
            }
            return _seguracaContextoDados;
        }
    }
}