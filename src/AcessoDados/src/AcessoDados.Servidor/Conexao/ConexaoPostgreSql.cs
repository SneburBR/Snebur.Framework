//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Snebur;
//using Snebur.Utilidade;
//using Snebur.Dominio;
//using System.Data;
//using System.Configuration;
//using Npgsql;
//using System.Data.Common;
//using Snebur.AcessoDados.Estrutura;

//namespace Snebur.AcessoDados
//{
//    internal class ConexaoPostgreSql : BaseConexao
//    {

//        public ConexaoPostgreSql(ContextoDados contexto, ConnectionStringSettings connectionString) : base(contexto, connectionString)
//        {
//        }

//        internal protected override DbConnection RetornarNovaConexao()
//        {
//            return new NpgsqlConnection(this.ConnectionString.ConnectionString);
//        }

//        internal protected override DbCommand RetornarNovoComando(string sql, List<DbParameter> parametros, DbConnection conexao)
//        {
//            var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)conexao);
//            if (parametros != null)
//            {
//                foreach (var parametro in parametros)
//                {
//                    cmd.Parameters.Add(parametro);
//                }
//            }
//            return cmd;
//        }

//        internal protected override DbCommand RetornarNovoComando(string sql, List<DbParameter> parametros, DbConnection conexao, DbTransaction transacao)
//        {
//            var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)conexao, (NpgsqlTransaction)transacao);
//            if (parametros != null)
//            {
//                foreach (var parametro in parametros)
//                {
//                    cmd.Parameters.Add(parametro);
//                }
//            }
//            return cmd;
//        }

//        internal protected override DbDataAdapter RetornarNovoDataAdapter(DbCommand cmd)
//        {
//            return new NpgsqlDataAdapter((NpgsqlCommand)cmd);
//        }

//        internal protected override DbParameter RetornarNovoParametro(EstruturaCampo estruturaCampo, string nomeParametro, object valor)
//        {
//            var parametro = new NpgsqlParameter(nomeParametro, estruturaCampo.Tipo);
//            parametro.Value = valor;
//            parametro.IsNullable = estruturaCampo.AceitaNulo;
//            return parametro;
//        }

//        protected internal override void TestarConexao()
//        {
//            try
//            {
//                var hora = this.RetornarDataHora();
//            }
//            catch (Exception erro)
//            {
//                throw new Erro(String.Format("Não foi possivel conectar o banco de dados"), erro);
//            }
//        }

//        protected internal override DateTime RetornarDataHora(bool utc = true)
//        {
//            var funcaoDataHora = (!utc) ? " NOW()" : " CAST(NOW() at time zone 'utc' AS timestamp)";
//            return Convert.ToDateTime(this.RetornarValorScalar(String.Format(" SELECT {0} ", funcaoDataHora), null));
//        }
//    }
//}