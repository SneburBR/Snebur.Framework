//using System.Data.Entity;
//using System.Data.Entity.Migrations;

//namespace Snebur.AcessoDados.Migracao
//{
//    public class InicializarContexto
//    {
//        private static object Bloqueio = new object();

//        private static bool Inicializado { get; set; } = false;

//        public static void Inicializar<TContexto, TConfiguracao>()
//            where TContexto : BaseContextoEntity
//            where TConfiguracao : DbMigrationsConfiguration<TContexto>, new()
//        {
//            lock (InicializarContexto.Bloqueio)
//            {
//                if (!InicializarContexto.Inicializado)
//                {
//                    Database.SetInitializer(new MigrateDatabaseToLatestVersion<TContexto, TConfiguracao>());
//                    InicializarContexto.Inicializado = true;
//                }
//            }
//        }
//    }
//}
