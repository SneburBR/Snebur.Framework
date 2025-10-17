using Snebur.AcessoDados.Estrutura;
using System.Data;
using System.Data.Common;

namespace Snebur.AcessoDados;

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

    internal protected abstract DbCommand RetornarNovoComando(
        string sql,
        IReadOnlyCollection<ParametroInfo>? parametros,
        DbConnection conexao);

    internal protected abstract DbCommand RetornarNovoComando(
        string sql,
        IReadOnlyCollection<ParametroInfo>? parametros,
        DbConnection conexao,
        DbTransaction transacao);

    internal protected abstract DbDataAdapter RetornarNovoDataAdapter(DbCommand cmd);

    internal protected abstract DbParameter RetornarNovoParametro(
        string nomeParametro,
        SqlDbType sqlDbType,
        int? size,
        object valor);

    internal protected abstract DbParameter RetornarNovoParametro(
        EstruturaCampo estruturaCampo,
        string nomeParametro,
        object? valor);

    internal protected abstract DateTime RetornarDataHora(bool utc = true);

    internal BaseContextoDados ContextoDados { get; }

    internal ParametroInfo RetornarParametroInfo(EstruturaCampo estruturaCampo,
                                                 string nomeParametro,
                                                 object? valor)
    {
        return ParametroInfo.Create(this, estruturaCampo, nomeParametro, valor);

    }

    #region IConexaoBancoDados

    internal DataTable RetornarDataTable(
        string sql,
        IReadOnlyCollection<ParametroInfo>? parametros)
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

    internal DataTable RetornarDataTableNormal(
        string sql,
        IReadOnlyCollection<ParametroInfo>? parametros)
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
                if(erroInterno is ErroExecutarSql)
                {
                    throw;
                }
                throw new ErroExecutarSql(sql, parametros, null, erroInterno);
            }
            finally
            {
                conexao.Close();
            }
        }
        return dt;
    }

    internal T RetornarValorScalar<T>(
        string sql, 
        IReadOnlyCollection<ParametroInfo>? parametros)
    {
        var valorScalar = this.RetornarValorScalar(sql, parametros);
        return ConverterUtil.Para<T>(valorScalar);

    }
    internal object? RetornarValorScalar(
        string sql,
        IReadOnlyCollection<ParametroInfo>? parametros)
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

    internal object? RetornarValorScalarNormal(
        string sql,
        IReadOnlyCollection<ParametroInfo>? parametros)
    {
        this.EscreverSaida(parametros, sql);

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
                        var valorEscalar = cmd.ExecuteScalar();
                        if (valorEscalar == DBNull.Value)
                        {
                            return null;
                        }
                        return valorEscalar;
                    }
                }
            }
            catch (Exception erroInterno)
            {
                throw new ErroExecutarSql(sql, parametros, null, erroInterno);
            }
            finally
            {
                conexao.Close();
            }
        }
    }

    internal int ExecutarComando(string sql,
        IReadOnlyCollection<ParametroInfo>? parametros)
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

    private int ExecutarComandoNormal(string sql, IReadOnlyCollection<ParametroInfo>? parametros)
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

    private int ExecutarComandoTransacao(
        string sql,
        IReadOnlyCollection<ParametroInfo>? parametrosInfo)
    {
        this.EscreverSaida(parametrosInfo, sql);

        var conexao = this.ContextoDados.ConexaoAtual;
        var transacao = this.ContextoDados.TransacaoAtual;

        Guard.NotNull(conexao);
        Guard.NotNull(transacao);

        using (var cmd = this.RetornarNovoComando(sql,
            parametrosInfo,
            conexao,
            transacao))
        {
            return cmd.ExecuteNonQuery();
        }
    }
    private void EscreverSaida(IReadOnlyCollection<ParametroInfo>? parametros, string sql)
    {
        DepuracaoUtil.EscreverSaida(this.ContextoDados, parametros, sql);
    }

    #region Transação

    private DataTable RetornarDataTransacao(
        string sql,
        IReadOnlyCollection<ParametroInfo>? parametros)
    {
        this.EscreverSaida(parametros, sql);

        var dt = new DataTable();
        var conexao = this.ContextoDados.ConexaoAtual;
        var transacao = this.ContextoDados.TransacaoAtual;

        Guard.NotNull(conexao);
        Guard.NotNull(transacao);

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

    private object? RetornarValorScalarTransacao(string sql,
                                                IReadOnlyCollection<ParametroInfo>? parametros)
    {
        this.EscreverSaida(parametros, sql);

        var conexao = this.ContextoDados.ConexaoAtual;
        var transacao = this.ContextoDados.TransacaoAtual;

        Guard.NotNull(conexao);
        Guard.NotNull(transacao);

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
