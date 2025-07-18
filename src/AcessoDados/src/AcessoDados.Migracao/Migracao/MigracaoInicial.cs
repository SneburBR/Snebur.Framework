using Snebur.Dominio;
using Snebur.BancoDados;
using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using System.Collections.Generic;
using System.Configuration;

using System.IO;
using System.Linq;
using System.Reflection;

#if NET9_0_OR_GREATER
using Microsoft.Data.SqlClient;
#endif
#if NET48
using System.Data.SqlClient;
#endif

namespace Snebur.Migracao
{
    //ESSE MIGRATION DEVE SER ADICIONADO ANTES DO PRIMEIRO CREATE TABLE

    public class MigracaoInicial
    {

        //public const string CAMINHO_BANCO_DADOS = @"C:\Program Files\Microsoft SQL Server\MSSQL13.SQLEXPRESS\MSSQL\DATA\";
        //public const string CAMINHO_BANCO_DADOS = @"C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\";

        public static List<string> RetonarSqls(Assembly assemblyEntidades, string nomeFonteDados)
        {
            var sqls = new List<string>();

            var atributosTabelas = assemblyEntidades.GetTypes().Where(x => x.IsSubclassOf(typeof(Entidade))).
                                            Select(x => x.GetCustomAttribute<TabelaAttribute>(false)).Where(x => x != null &&
                                                                                                                 x.GetType() != typeof(TabelaAttribute)).Distinct().ToList();

            var gruosArquivoDados = atributosTabelas.Select(x => x.GrupoArquivoDados).Where(x => x != null).ToList();

            var gruosArquivoIndioces = atributosTabelas.Select(x => x.GrupoArquivoIndices).Where(x => x != null).ToList();

            var gruposArquivos = new List<string>();
            gruposArquivos.AddRange(gruosArquivoDados);
            gruposArquivos.AddRange(gruosArquivoIndioces);

          

            var construtor = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings[nomeFonteDados].ConnectionString);
            var nomeBancoDados = construtor.InitialCatalog;
            foreach (var grupoArquivo in gruposArquivos)
            {
                var nomeGrupoArquivo = TextoUtil.RetornarPrimeiraLetraMaiuscula(grupoArquivo);
                sqls.AddRange(MigracaoInicial.RetornarSqlsAdicioanarFileGroup(nomeFonteDados, nomeBancoDados, nomeGrupoArquivo));
            }
            return sqls;

        }
        private static List<string> RetornarSqlsAdicioanarFileGroup(string nomeFonteDados, string nomeBancoDados, string nomeGrupoArquivo)
        {
            var sqls = new List<string>();
            //var nomeGrupoArquivo = $"{nomeFonteDados}_ {grupoArquivo}";

            var nomeArquivo = $"{nomeBancoDados}_{nomeGrupoArquivo}.mdf";
            var diretorioBanco = RetornarDiretorioBancoDados(nomeFonteDados);
            var caminhoArquivo = Path.Combine(diretorioBanco, nomeArquivo);

            var sql = $" IF NOT EXISTS(select * from sys.filegroups where name = '{nomeGrupoArquivo}')"
                            + $"\n      ALTER DATABASE[{nomeBancoDados}] ADD FILEGROUP [{nomeGrupoArquivo}];";

            sqls.Add(sql);
            sqls.Add(MigracaoInicial.RetornarSqlAdicioanarFileGroup(nomeBancoDados, nomeGrupoArquivo, caminhoArquivo));
            return sqls;
        }

        private static string RetornarSqlAdicioanarFileGroup(string nomeBancoDados, string nomeGrupoArquivo, string caminhoArquivo)
        {

            return $" IF NOT EXISTS(select * from sys.sysfiles where name = '{nomeGrupoArquivo}' )"
                             + $"\n   ALTER DATABASE [{nomeBancoDados}]"
                             + $"\n    ADD FILE ( "
                             + $"\n              NAME = [{nomeGrupoArquivo}], "
                             + $"\n              FILENAME = '{caminhoArquivo}', "
                             + $"\n              SIZE = 8MB, "
                             + $"\n              FILEGROWTH = 64MB  ) "
                             + $"\n TO FILEGROUP [{nomeGrupoArquivo}];"
                             + $"\n GO \n";

        }

        private static string RetornarDiretorioBancoDados(string nomeFonteDados)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[nomeFonteDados].ConnectionString;
            var construtor = new SqlConnectionStringBuilder(connectionString);
            var nomeBancoDados = construtor.InitialCatalog;
            construtor.InitialCatalog = "master";
            var conectionStringMaster = construtor.ToString();

            var sql = $"SELECT TOP 1 DB.name As NomeBancoDados, F.physical_name  As CaminhoBanco FROM sys.databases DB JOIN sys.master_files F ON DB.database_id=F.database_id WHERE DB.name = '{nomeBancoDados}' OR DB.name = 'master' ";
            var conexao = new Conexao(conectionStringMaster);
            var bancoCaminhos = conexao.Mapear<BancoCaminho>(sql);
            return Path.GetDirectoryName(bancoCaminhos.Single().CaminhoBanco);
        }

        public class BancoCaminho
        {
            public string NomeBancoDados { get; set; }
            public string CaminhoBanco { get; set; }
        }
    }
}

