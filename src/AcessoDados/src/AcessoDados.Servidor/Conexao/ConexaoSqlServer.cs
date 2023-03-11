using System;
using System.Collections.Generic;
using System.Data.Common;
using Snebur.AcessoDados.Estrutura;

#if NET7_0
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif

namespace Snebur.AcessoDados
{
    internal class ConexaoSqlServer : BaseConexao
    {

        public ConexaoSqlServer(BaseContextoDados contextoDados, string connectionString) : base(contextoDados, connectionString)
        {
        }

        internal protected override DbConnection RetornarNovaConexao()
        {
            return new SqlConnection(this.ConnectionString);
        }

        internal protected override DbCommand RetornarNovoComando(string sql, List<DbParameter> parametros, DbConnection conexao)
        {
            var cmd = new SqlCommand(sql, (SqlConnection)conexao);
            if (parametros != null)
            {
                foreach (var parametro in parametros)
                {
                    cmd.Parameters.Add(parametro);
                }
            }
            return cmd;
        }

        internal protected override DbCommand RetornarNovoComando(string sql, List<DbParameter> parametros, DbConnection conexao, DbTransaction transacao)
        {
            var cmd = new SqlCommand(sql, (SqlConnection)conexao, (SqlTransaction)transacao);
            if (parametros != null)
            {
                foreach (var parametro in parametros)
                {
                    cmd.Parameters.Add(parametro);
                }
            }
            return cmd;
        }

        internal protected override DbDataAdapter RetornarNovoDataAdapter(DbCommand cmd)
        {
            return new SqlDataAdapter((SqlCommand)cmd);
        }


        internal protected override DbParameter RetornarNovoParametro(EstruturaCampo estruturaCampo, string nomeParametro, object valor)
        {
            var parametro = new SqlParameter(nomeParametro, estruturaCampo.TipoSql)
            {
                Value = valor ?? DBNull.Value,
                IsNullable = estruturaCampo.IsAceitaNulo
            };
            //parametro.Size = estruturaCampo.TamanhoMaximoString;
            return parametro;
        }

        //internal protected override DbParameter RetornarNovoParametro(SqlDbType tipoSql, bool aceitaNulo, string nomeParametro, object valor)
        //{
        //    var parametro = new SqlParameter(nomeParametro, tipoSql);
        //    parametro.Value = valor;
        //    parametro.IsNullable = estruturaCampo.AceitaNulo;
        //    //parametro.Size = estruturaCampo.TamanhoMaximoString;
        //    return parametro;
        //}

        protected internal override void TestarConexao()
        {
            try
            {
                var hora = this.RetornarDataHora();
            }
            catch (Exception erro)
            {
                throw new Erro(String.Format("Não foi possível conectar o banco de dados"), erro);
            }
        }

        protected internal override DateTime RetornarDataHora(bool utc = true)
        {
            var funcaoDataHora = (utc) ? "GETUTCDATE()" : " GETDATE()";
            return Convert.ToDateTime(this.RetornarValorScalar(String.Format(" SELECT {0} ", funcaoDataHora), null));
        }
    }
}
