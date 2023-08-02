using Snebur.AcessoDados.Estrutura;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

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

        internal protected abstract DbCommand RetornarNovoComando(string sql, List<DbParameter> parametros, DbConnection conexao);

        internal protected abstract DbCommand RetornarNovoComando(string sql, List<DbParameter> parametros, DbConnection conexao, DbTransaction transacao);

        internal protected abstract DbDataAdapter RetornarNovoDataAdapter(DbCommand cmd);

        internal protected abstract DbParameter RetornarNovoParametro(EstruturaCampo estruturaCampo, string nomeParametro, object valor);

        internal protected abstract DateTime RetornarDataHora(bool utc = true);

        internal BaseContextoDados ContextoDados { get; }

        internal DbParameter RetornarNovoParametro(EstruturaCampo estruturaCampo, object valor)
        {
            return this.RetornarNovoParametro(estruturaCampo, estruturaCampo.NomeParametro, valor);
        }
        #region IConexaoBancoDados

        internal DataTable RetornarDataTable(string sql, List<DbParameter> parametros)
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

        internal DataTable RetornarDataTableNormal(string sql, List<DbParameter> parametros)
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
                            using (var ad = this.RetornarNovoDataAdapter(cmd))
                            {
                                ad.Fill(dt);
                            }
                        }
                    }
                }
                catch(Exception erroInterno)
                {
                    throw new ErroConsultaSql(String.Format("Erro ao preencher o dataTable DataAdpater: Sql {0} ", sql), erroInterno);
                }
                finally
                {
                    conexao.Close();
                }
            }
            return dt;
        }

        internal object RetornarValorScalar(string sql, List<DbParameter> parametros)
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

        internal object RetornarValorScalarNormal(string sql, List<DbParameter> parametros)
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
                        using (var cmd = this.RetornarNovoComando(sql, parametros, conexao, transacao))
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

        internal int ExecutarComando(string sql, List<DbParameter> parametros)
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

        private int ExecutarComandoNormal(string sql, List<DbParameter> parametros)
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

        private int ExecutarComandoTransacao(string sql, List<DbParameter> parametros)
        {
            this.EscreverSaida(parametros, sql);

            var conexao = this.ContextoDados.ConexaoAtual;
            var transacao = this.ContextoDados.TransacaoAtual;
            using (var cmd = this.RetornarNovoComando(sql, parametros, conexao, transacao))
            {
                return cmd.ExecuteNonQuery();
            }
        }
        private void EscreverSaida(List<DbParameter> parametros, string sql)
        {
            DepuracaoUtil.EscreverSaida(this.ContextoDados, parametros, sql);
        }

        #region Transação

        private DataTable RetornarDataTransacao(string sql, List<DbParameter> parametros)
        {
            this.EscreverSaida(parametros, sql);

            var dt = new DataTable();
            var conexao = this.ContextoDados.ConexaoAtual;
            var transacao = this.ContextoDados.TransacaoAtual;

            using (var cmd = this.RetornarNovoComando(sql, parametros, conexao, transacao))
            {
                using (var ad = this.RetornarNovoDataAdapter(cmd))
                {
                    ad.Fill(dt);
                    return dt;
                }
            }
        }

        private object RetornarValorScalarTransacao(string sql, List<DbParameter> parametros)
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
}