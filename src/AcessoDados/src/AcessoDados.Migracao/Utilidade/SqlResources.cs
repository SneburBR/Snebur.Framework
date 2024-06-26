using System;
using System.Reflection;

namespace Snebur.AcessoDados.Migracao
{
    public static class SqlResources
    {
        private static Assembly _entryAssembly;
        public static Assembly EntryAssembly
        {
            get => SqlResources._entryAssembly ?? Assembly.GetEntryAssembly();
            set => SqlResources._entryAssembly = value;
        }
         
        public static string RetornarSqlResouurce(string nome)
        {
            var assembly = SqlResources.EntryAssembly;
            var caminhoResource = $"{assembly.GetName().Name}.Resources.{ nome}";
            var resourceStream = assembly.GetManifestResourceStream(caminhoResource);
            if (resourceStream == null)
            {
                throw new Exception($"Não foi possível encontrar o arquivo de recurso. {nome}");
            }

            using (var streamReader = new System.IO.StreamReader(resourceStream))
            {
                return streamReader.ReadToEnd();
            }
        }

        internal static string RetornarSqlUpdateDescricaoEmpresaDuplicada(string nomeTabela,
                                                                          string nomeCampoId)
        {
            var sql = SqlResources.RetornarSqlResouurce("TB_Empresa_Descricao_duplicada.sql");
            return sql.Replace("{{NOME_TABELA}}", nomeTabela).
                       Replace("{{NOME_CAMPO_ID}}", nomeCampoId);
        }
    }
}
