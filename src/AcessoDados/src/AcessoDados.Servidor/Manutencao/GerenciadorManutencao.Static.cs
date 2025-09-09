namespace Snebur.AcessoDados.Manutencao;

internal partial class GerenciadorManutencao
{
    private static bool Inicializado { get; set; }
    private static object Bloqueio { get; set; } = new object();

    internal static void Inicializar(BaseContextoDados contexto)
    {
        lock (Bloqueio)
        {
            if (!Inicializado)
            {
                using (var manutencao = new GerenciadorManutencao(contexto))
                {
                    manutencao.Executar();
                }
                Inicializado = true;
            }
        }
    }
}
