namespace Snebur.AcessoDados
{
    internal partial class GerenciadorMigracao
    {
        private static bool Inicializado { get; set; }
        private static object Bloqueio { get; set; } = new object();

        internal static void Inicializar(BaseContextoDados contexto)
        {
            lock (GerenciadorMigracao.Bloqueio)
            {
                if (!GerenciadorMigracao.Inicializado)
                {
                    using (var migration = new GerenciadorMigracao(contexto))
                    {
                        migration.Migrar();
                    }
                    GerenciadorMigracao.Inicializado = true;
                }
            }
        }
    }
}
