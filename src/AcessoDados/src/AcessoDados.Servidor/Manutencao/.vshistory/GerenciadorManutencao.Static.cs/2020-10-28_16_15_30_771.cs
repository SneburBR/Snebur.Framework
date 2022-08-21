namespace Snebur.AcessoDados.Manutencao
{
    internal partial class GerenciadorManutencao
    {
        private static bool Inicializado { get; set; }
        private static object Bloqueio { get; set; } = new object();

        internal static void Inicializar(ContextoDados contexto)
        {
            lock (GerenciadorManutencao.Bloqueio)
            {
                if (!GerenciadorManutencao.Inicializado)
                {
                    using (var manutencao = new GerenciadorManutencao(contexto))
                    {
                        manutencao.Executar();
                    }
                    GerenciadorManutencao.Inicializado = true;
                }
            }
        }
    }
}
