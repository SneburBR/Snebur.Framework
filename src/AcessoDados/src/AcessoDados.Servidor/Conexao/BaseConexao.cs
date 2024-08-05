using Snebur.AcessoDados.Estrutura;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
#if NET6_0_OR_GREATER
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif

namespace Snebur.AcessoDados
{
    internal abstract class BaseConexao
    //: IConexaoBancoDados
    {
        internal protected string ConnectionString { get; set; }
        private static readonly object _bloqueio = new object();
        private static bool _isTesteConexaoPendente = true;

        public BaseConexao(BaseContextoDados contexto,
                           string connectionString)
        {
            this.ConnectionString = connectionString;
            this.ContextoDados = contexto;

            if (_isTesteConexaoPendente)
            {
                lock (_bloqueio)
                {
                    if (_isTesteConexaoPendente)
                    {
                        this.TestarConexao();
                        _isTesteConexaoPendente = false;
                    }
                }
            }
        }

        internal protected abstract void TestarConexao();

        internal protected abstract DbConnection RetornarNovaConexao();

        internal protected abstract DbCommand RetornarNovoComando(string sql, List<ParametroInfo> parametros, DbConnection conexao);

        internal protected abstract DbCommand RetornarNovoComando(string sql,
                                                                  List<ParametroInfo> parametros,
                                                                  DbConnection conexao,
                                                                  DbTransaction transacao);

        internal protected abstract DbDataAdapter RetornarNovoDataAdapter(DbCommand cmd);

        internal protected abstract DbParameter RetornarNovoParametro(string nomeParametro,
                                                                      SqlDbType sqlDbType,
                                                                      int? size,
                                                                      object valor);

        internal protected abstract DbParameter RetornarNovoParametro(EstruturaCampo estruturaCampo,
                                                                      string nomeParametro,
                                                                      object valor);

        internal protected abstract DateTime RetornarDataHora(bool utc = true);

        internal BaseContextoDados ContextoDados { get; }

        internal ParametroInfo RetornarParametroInfo(EstruturaCampo estruturaCampo,
                                                     string nomeParametro,
                                                     object valor)
        {
            return new ParametroInfo(this)
            {
                EstruturaCampo = estruturaCampo,
                ParameterName = nomeParametro ?? estruturaCampo.NomeParametro,
                Value = valor
            };
        }

        #region IConexaoBancoDados

        internal DataTable RetornarDataTable(string sql,
                                             List<ParametroInfo> parametros)
        {
            if (this.ContextoDados.IsExisteTransacao)
            {
                return this.RetornarDataTransacao(sql, parametros);
            }
            else
            {
                return this.RetornarDataTableNormal(sql, parametros);
            }
        }

        internal DataTable RetornarDataTableNormal(string sql, List<ParametroInfo> parametros)
        {
            this.EscreverSaida(parametros, sql);

            var dt = new DataTable();
            using (var conexao = this.RetornarNovaConexao())
            {
                conexao.Open();
                try
                {
                    using (var transacao = conexao.BeginTransaction(ConfiguracaoAcessoDados.IsolamentoLevelConsultaPadrao))
                    {
                        using (var cmd = this.RetornarNovoComando(sql,
                                                                  parametros,
                                                                  conexao,
                                                                  transacao))
                        {
                            DepuracaoUtil.EscreverSaida(this.ContextoDados,
                                                        parametros,
                                                        sql);

                            using (var ad = this.RetornarNovoDataAdapter(cmd))
                            {
                                ad.Fill(dt);
                            }
                        }
                    }
                }
                catch (Exception erroInterno)
                {
                    throw new ErroConsultaSql($"Erro ao preencher o dataTable DataAdpater: Sql {sql} ", erroInterno);
                }
                finally
                {
                    conexao.Close();
                }
            }
            return dt;
        }

        internal T RetornarValorScalar<T>(string sql, List<ParametroInfo> parametros)
        {
            var valorScalar = this.RetornarValorScalar(sql, parametros);
            return ConverterUtil.Para<T>(valorScalar);

        }
        internal object RetornarValorScalar(string sql, List<ParametroInfo> parametros)
        {
            if (this.ContextoDados.IsExisteTransacao)
            {
                return this.RetornarValorScalarTransacao(sql, parametros);
            }
            else
            {
                return this.RetornarValorScalarNormal(sql, parametros);
            }
        }

        internal object RetornarValorScalarNormal(string sql, List<ParametroInfo> parametros)
        {
            this.EscreverSaida(parametros, sql);

            object valorEscalor;
            using (var conexao = this.RetornarNovaConexao())
            {
                conexao.Open();
                try
                {
                    using (var transacao = conexao.BeginTransaction(ConfiguracaoAcessoDados.IsolamentoLevelConsultaPadrao))
                    {
                        using (var cmd = this.RetornarNovoComando(sql,
                                                                  parametros,
                                                                  conexao,
                                                                  transacao))
                        {
                            valorEscalor = cmd.ExecuteScalar();
                        }
                    }
                }
                catch (Exception erroInterno)
                {
                    throw new ErroConsultaSql(String.Format("Erro ao executar valor scalar do SQL {0} ", sql), erroInterno);
                }
                finally
                {
                    conexao.Close();
                }
            }
            if (valorEscalor == DBNull.Value)
            {
                valorEscalor = null;
            }
            return valorEscalor;
        }

        internal int ExecutarComando(string sql, List<ParametroInfo> parametros)
        {
            if (this.ContextoDados.IsExisteTransacao)
            {
                return this.ExecutarComandoTransacao(sql, parametros);
            }
            else
            {
                return this.ExecutarComandoNormal(sql, parametros);
            }
        }

        private int ExecutarComandoNormal(string sql, List<ParametroInfo> parametros)
        {
            this.EscreverSaida(parametros, sql);

            using (var conexao = this.RetornarNovaConexao())
            {
                try
                {
                    conexao.Open();
                    using (var cmd = this.RetornarNovoComando(sql, parametros, conexao))
                    {
                        return cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private int ExecutarComandoTransacao(string sql, List<ParametroInfo> parametrosInfo)
        {
            this.EscreverSaida(parametrosInfo, sql);

            var conexao = this.ContextoDados.ConexaoAtual;
            var transacao = this.ContextoDados.TransacaoAtual;
            using (var cmd = this.RetornarNovoComando(sql, parametrosInfo, conexao, transacao))
            {
                return cmd.ExecuteNonQuery();
            }
        }
        private void EscreverSaida(List<ParametroInfo> parametros, string sql)
        {
            DepuracaoUtil.EscreverSaida(this.ContextoDados, parametros, sql);
        }

        #region Transação

        private DataTable RetornarDataTransacao(string sql, List<ParametroInfo> parametros)
        {
            this.EscreverSaida(parametros, sql);

            var dt = new DataTable();
            var conexao = this.ContextoDados.ConexaoAtual;
            var transacao = this.ContextoDados.TransacaoAtual;

            using (var cmd = this.RetornarNovoComando(sql, parametros, conexao, transacao))
            {
                DepuracaoUtil.EscreverSaida(this.ContextoDados,
                                            parametros,
                                            sql);

                using (var ad = this.RetornarNovoDataAdapter(cmd))
                {
                    ad.Fill(dt);
                    return dt;
                }
            }
        }

        private object RetornarValorScalarTransacao(string sql,
                                                    List<ParametroInfo> parametros)
        {
            this.EscreverSaida(parametros, sql);

            var conexao = this.ContextoDados.ConexaoAtual;
            var transacao = this.ContextoDados.TransacaoAtual;
            using (var cmd = this.RetornarNovoComando(sql, parametros, conexao, transacao))
            {
                var valorEscalar = cmd.ExecuteScalar();
                if (valorEscalar == DBNull.Value)
                {
                    return null;
                }
                return valorEscalar;
            }
        }

        #endregion

        #endregion
    }

    public class ParametroInfo
    {
        private BaseConexao BaseConexao;
        public string ParameterName { get; set; }
        public SqlDbType? SqlDbType { get; set; }
        public int? Size;
        public object Value { get; set; }
        internal EstruturaCampo EstruturaCampo { get; set; }

        public ParametroInfo()
        {
        }

        internal ParametroInfo(BaseConexao baseConexao)
        {
            this.BaseConexao = baseConexao;
        }

        public ParametroInfo(string nomeParametro, object value) : this(nomeParametro, SqlUtil.GetBetterSqlDbType(value), value)
        {

        }
        public ParametroInfo(string nomeParametro,
                             SqlDbType sqlType,
                             object value) : this(nomeParametro, sqlType, null, value)
        {

        }

        public ParametroInfo(string nomeParametro,
                             SqlDbType sqlType,
                             int? size,
                             object value)
        {
            this.ParameterName = nomeParametro;
            this.Value = value;
            this.SqlDbType = sqlType;
            this.Size = size;
        }

        internal DbParameter GetDbParameter()
        {
            if (this.SqlDbType.HasValue)
            {
                return new SqlParameter(this.ParameterName, this.SqlDbType.Value)
                {
                    Value = this.Value ?? DBNull.Value,
                    Size = this.Size ?? 0,
                    IsNullable = this.Value == null
                };
                //return this.BaseConexao.RetornarNovoParametro(this.ParameterName,
                //                                              this.SqlDbType.Value,
                //                                              this.Size,
                //                                              this.Value);
            }

            return this.BaseConexao.RetornarNovoParametro(this.EstruturaCampo,
                                                           this.ParameterName,
                                                           this.Value);

        }



    }
}