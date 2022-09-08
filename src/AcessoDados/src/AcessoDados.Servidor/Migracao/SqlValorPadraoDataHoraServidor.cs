using Snebur.AcessoDados.Estrutura;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Snebur.AcessoDados
{
    internal class SqlValorPadraoDataHoraServidor : SqlMigracao
    {
        internal PropertyInfo Propriedade { get; set; }
        internal bool DataHoraUTC { get; set; }
        internal string CampoChavePrimaria { get; set; }
        internal string CampoDataHoraPadrao { get; set; }

        internal SqlValorPadraoDataHoraServidor(EstruturaEntidade estruturaEntidade, PropertyInfo propriedade, bool dataHoraUTc) : base(estruturaEntidade, new List<PropertyInfo> { propriedade })
        {
            this.Propriedade = propriedade;
            this.DataHoraUTC = dataHoraUTc;
            this.CampoChavePrimaria = this.EstruturaEntidade.EstruturaCampoChavePrimaria.NomeCampo;
            this.CampoDataHoraPadrao = this.Campos.Single();
        }

        protected override string RetornarSql_PostgreSQL()
        {
            var nomeFuncao = this.RetornarNomeFuncao();
            var nomeGatilho = this.RetornarNomeGatilho();

            var funcaoDataHora = (!this.DataHoraUTC) ? " statement_timestamp()" : " CAST(statement_timestamp() at time zone 'utc' AS timestamp)";

            var sb = new StringBuilder();

            sb.AppendLine(String.Format("CREATE OR REPLACE FUNCTION {0}()", nomeFuncao));
            sb.AppendLine(String.Format("RETURNS trigger AS"));
            sb.AppendLine(String.Format("$$BEGIN"));
            sb.AppendLine(String.Format("NEW.\"{0}\"=(SELECT {1});", this.CampoDataHoraPadrao, funcaoDataHora));
            sb.AppendLine(String.Format(" RETURN NEW;"));
            sb.AppendLine("END;$$");
            sb.AppendLine("LANGUAGE plpgsql VOLATILE;");

            sb.AppendLine();

            sb.AppendLine(String.Format("CREATE TRIGGER {0}  BEFORE INSERT ON", nomeGatilho));
            sb.AppendLine(String.Format("\"{0}\" FOR EACH ROW EXECUTE PROCEDURE", this.NomeTabela));
            sb.AppendLine(String.Format("{0}();", nomeFuncao));
            sb.AppendLine(String.Format(""));

            return sb.ToString();
        }

        protected override string RetornarSql_SqlServer()
        {
            var funcaoHora = (this.DataHoraUTC) ? " GETUTCDATE() " : " GETDATE() ";
            var nomeGatilho = this.RetornarNomeGatilho();
            var sb = new StringBuilder();

            sb.AppendLine(String.Format(" IF NOT EXISTS ( select * from sys.triggers where name= N'{0}') ", nomeGatilho));
            sb.AppendLine("BEGIN");
            sb.AppendLine(" EXEC ('");
            sb.AppendLine(String.Format("      CREATE TRIGGER [{0}].[{1}]       ", this.Schema, nomeGatilho));
            sb.AppendLine(String.Format("      ON [{0}].[{1}] ", this.Schema, this.NomeTabela));
            sb.AppendLine("                    AFTER INSERT");
            sb.AppendLine("     AS BEGIN ");
            sb.AppendLine(String.Format("  UPDATE [{0}].[{1}]  SET {2} = {4}  FROM Inserted AS NovoRegistro WHERE  [{0}].[{1}].[{3}] = NovoRegistro.[{3}] ", this.Schema, this.NomeTabela, this.CampoDataHoraPadrao, this.CampoChavePrimaria, funcaoHora));
            sb.AppendLine("       END ");
            sb.AppendLine("    ')");
            sb.AppendLine("END");

            return sb.ToString();
        }

        public string RetornrSqlExisteGatilho()
        {
            var nomeGatilho = this.RetornarNomeGatilho();
            return String.Format("SELECT COUNT(*) FROM pg_trigger WHERE tgname='{0}';", nomeGatilho);
        }

        private string RetornarNomeGatilho()
        {
            return String.Format("Gatilho_{0}_{1}_DataHoraPadrao", this.NomeTabela, this.CampoDataHoraPadrao).ToLower();
        }

        private string RetornarNomeFuncao()
        {
            return String.Format("Funcao_Gatilho_{0}_{1}_DataHoraPadrao", this.NomeTabela, this.CampoDataHoraPadrao).ToLower();
        }
    }
}