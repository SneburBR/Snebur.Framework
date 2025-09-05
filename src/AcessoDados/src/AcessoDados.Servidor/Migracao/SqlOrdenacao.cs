using Snebur.AcessoDados.Estrutura;
using System.Text;

namespace Snebur.AcessoDados
{
    internal class SqlOrdenacao : SqlMigracao
    {

        internal PropertyInfo Propriedade { get; set; }
        internal EnumOrdenacaoNovoRegistro OrdenacaoNovoRegistro { get; set; }
        internal string CampoChavePrimaria { get; set; }
        internal string CampoOrdenacao { get; set; }

        internal SqlOrdenacao(EstruturaEntidade estruturaEntidade, 
                              PropertyInfo propriedade, 
                              EnumOrdenacaoNovoRegistro ordencaoNovoRegistro) : base(estruturaEntidade, new List<PropertyInfo>() { propriedade })
        {
            this.Propriedade = propriedade;
            this.OrdenacaoNovoRegistro = ordencaoNovoRegistro;
            this.CampoChavePrimaria = this.EstruturaEntidade.EstruturaCampoChavePrimaria.NomeCampo;
            this.CampoOrdenacao = this.Campos.Single();
        }

        protected override string RetornarSql_PostgreSQL()
        {
            throw new ErroNaoImplementado();
        }

        protected override string RetornarSql_SqlServer()
        {
            var nomeGatilho = this.RetornarNomeGatilho();

            var sb = new StringBuilder();

            sb.AppendLine($"IF NOT EXISTS (SELECT * FROM DBO.SYSOBJECTS WHERE ID = OBJECT_ID(N'[{this.Schema}].[{nomeGatilho}]') AND OBJECTPROPERTY(ID, N'IsTrigger') = 1) ");
            sb.AppendLine("EXEC(' ");
            sb.AppendLine($"CREATE TRIGGER [{this.Schema}].[{nomeGatilho}] ");
            sb.AppendLine($" ON [{this.Schema}].[{this.NomeTabela}] FOR INSERT AS ");

            sb.AppendLine(" DECLARE @OrdenacaoAtual int");
            sb.AppendLine(" BEGIN ");
            sb.AppendLine(" SET NOCOUNT ON; ");
            sb.AppendLine(" BEGIN TRANSACTION ");

            sb.AppendLine(String.Format("SET @OrdenacaoAtual = (SELECT [{0}] FROM Inserted)", this.CampoOrdenacao));

            sb.AppendLine("IF (@OrdenacaoAtual = 0 OR @OrdenacaoAtual Is Null)");
            sb.AppendLine("BEGIN");

            if (this.OrdenacaoNovoRegistro == EnumOrdenacaoNovoRegistro.Fim)
            {
                sb.AppendLine($"UPDATE [{this.Schema}].[{this.NomeTabela}] SET [{this.CampoOrdenacao}] = (SELECT COALESCE(MAX([{this.CampoOrdenacao}] ), 0) + 10 AS ProximoOrdenacao FROM [{this.Schema}].[{this.NomeTabela}]) FROM Inserted  WHERE [{this.Schema}].[{this.NomeTabela}].[{this.CampoChavePrimaria}] = Inserted.[{this.CampoChavePrimaria}]");
            }
            else
            {
                sb.AppendLine($"UPDATE [{this.Schema}].[{this.NomeTabela}] SET [{this.CampoOrdenacao}] = (SELECT COALESCE(MIN([{this.CampoOrdenacao}] ), 0) - 10 AS ProximoOrdenacao FROM [{this.Schema}].[{this.NomeTabela}]) FROM Inserted  WHERE [{this.Schema}].[{this.NomeTabela}].[{this.CampoChavePrimaria}] = Inserted.[{this.CampoChavePrimaria}]");
            }
            sb.AppendLine("END");
            sb.AppendLine(" COMMIT ");
            sb.AppendLine(" END ')");

            return sb.ToString();
        }

        private string RetornarNomeGatilho()
        {
            return String.Format("Gatilho_{0}_Ordencao", this.NomeTabela);
        }
    }
}