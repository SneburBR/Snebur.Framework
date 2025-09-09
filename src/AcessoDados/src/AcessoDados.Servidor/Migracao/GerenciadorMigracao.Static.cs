namespace Snebur.AcessoDados;

internal partial class GerenciadorMigracao
{
    private static readonly HashSet<Type> Inicializados = new HashSet<Type>();
    private static object Bloqueio { get; set; } = new object();

    internal static void Inicializar(BaseContextoDados contexto)
    {
        //lock (GerenciadorMigracao.Bloqueio)
        //{
        //    if (!GerenciadorMigracao.Inicializados.Contains(contexto.GetType()))
        //    {
        //        using (var migration = new GerenciadorMigracao(contexto))
        //        {
        //            migration.Migrar();
        //        }
        //        GerenciadorMigracao.Inicializados.Add(contexto.GetType());
        //    }
        //}
    }
}
