using Snebur.AcessoDados.Estrutura;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Snebur.AcessoDados
{
    internal class SqlIndexarTextoCompleto : SqlMigracao
    {
        internal string NomeCatalogo { get; }
        internal string NomeChave { get; }
        internal string NomeCampos => String.Join(", ", this.Campos);

        internal SqlIndexarTextoCompleto(EstruturaEntidade estruturaEntidade, PropertyInfo propriedade) :
            base(estruturaEntidade, new List<PropertyInfo> { propriedade })
        {
            this.NomeCatalogo = this.RetornarNomeCatalogo();
            this.NomeChave = $"PK_{this.Schema}.{this.NomeTabela}";
        }
        protected override string RetornarSql_PostgreSQL()
        {
            throw new NotImplementedException();
        }

        protected override string RetornarSql_SqlServer()
        {
            var sqlCatalogo = this.RetornarSqlCriarCatalago();
            var sqlIndice = this.RetornarSqlIndiceTextoCompleto();

            return sqlCatalogo + sqlIndice;


        }

        public string RetornarSqlCriarCatalago()
        {
            var nomeCatalogo = this.RetornarNomeCatalogo();
            var sb = new StringBuilder();
            sb.AppendLine($" IF NOT EXISTS(select * from sys.fulltext_catalogs where name = '{nomeCatalogo}') ");
            sb.AppendLine($" CREATE FULLTEXT CATALOG[{nomeCatalogo}] WITH ACCENT_SENSITIVITY = OFF");
            //sb.AppendLine("  GO ");
            return sb.ToString(); ;
        }

        public string RetornarSqlIndiceTextoCompleto()
        {

            var sb = new StringBuilder();
            var sqlExiste = this.RetornarSqlExisteIndice();

            sb.AppendLine(sqlExiste);
            sb.AppendLine($"CREATE FULLTEXT INDEX ON {this.Schema}.{this.NomeTabela}   ({this.NomeCampos}) ");
            sb.AppendLine($" KEY INDEX[{this.NomeChave}] ");
            sb.AppendLine($" on([{this.NomeCatalogo}], FILEGROUP[{this.GrupoArquivoIndices}]); ");
            sb.AppendLine("");

            //sb.AppendLine(" GO ");

            return sb.ToString();

        }

        private string RetornarSqlExisteIndice()
        {
            var sb = new StringBuilder();

            sb.AppendLine(" IF  NOT  EXISTS( ");

            sb.AppendLine("  SELECT SCHEMA_NAME(t.schema_id) AS SchemaName, t.name AS TableName,  c.name AS FTCatalogName , f.name AS FileGroupName, i.name AS UniqueIdxName, cl.name AS ColumnName ");
            sb.AppendLine(" FROM  sys.tables t  INNER JOIN  sys.fulltext_indexes fi  ON  t.[object_id] = fi.[object_id]  INNER JOIN  sys.fulltext_index_columns ic ON  ic.[object_id] = t.[object_id] INNER JOIN sys.columns cl ");
            sb.AppendLine(" ON ic.column_id = cl.column_id AND ic.[object_id] = cl.[object_id] INNER JOIN  sys.fulltext_catalogs c  ON  fi.fulltext_catalog_id = c.fulltext_catalog_id INNER JOIN  sys.filegroups f ON   fi.data_space_id = f.data_space_id INNER JOIN  sys.indexes i ");
            sb.AppendLine(" ON   fi.unique_index_id = i.index_id AND fi.[object_id] = i.[object_id] ");
            sb.AppendLine($" WHERE SCHEMA_NAME(t.schema_id)  = '{this.Schema}' AND");
            sb.AppendLine($" t.name = '{this.NomeTabela}' and ");
            sb.AppendLine($" c.name = '{this.NomeCatalogo}' and");
            sb.AppendLine($" f.name = '{this.GrupoArquivoIndices}' and ");
            sb.AppendLine($" i.name = '{this.NomeChave}' and ");
            sb.AppendLine($" cl.name = '{this.NomeCampos}' ");
            sb.AppendLine(" )");


            return sb.ToString();
        }

        private string RetornarNomeCatalogo()
        {
            return $"{this.GrupoArquivoIndices}_Catalago";
        }
    }
}