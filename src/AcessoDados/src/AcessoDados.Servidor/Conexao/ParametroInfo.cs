using Snebur.AcessoDados.Estrutura;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Data.SqlClient;

namespace Snebur.AcessoDados;

public class ParametroInfo : IParametroInfo
{
    private BaseConexao? BaseConexao;
    public required string ParameterName { get; init; }
    public required int? Size { get; init; }
    public required object? Value { get; set; }
    public required SqlDbType SqlDbType { get; init; }
    internal EstruturaCampo? EstruturaCampo { get; set; }

    public ParametroInfo()
    {
    }

    public static ParametroInfo Create<TEntidade>(
       BaseContextoDados contextoDados,
       Expression<Func<TEntidade, object?>> expressaoPropriedade)
       where TEntidade : IEntidade
    {
        var estruturaEntidade = contextoDados.EstruturaBancoDados.RetornarEstruturaEntidade(typeof(TEntidade));
        var propriedade = ExpressaoUtil.RetornarPropriedade(expressaoPropriedade);
        var estruturaCampo = estruturaEntidade.RetornarEstruturaCampo(propriedade.Name);
        return Create(contextoDados.Conexao, estruturaCampo, estruturaCampo.NomeParametro, null);
    }

    internal static ParametroInfo Create(
        BaseConexao baseConexao,
        EstruturaCampo estruturaCampo,
        string nomeParametro,
        object? valor)
    {
        Guard.NotNullOrWhiteSpace(nomeParametro);

        return new ParametroInfo
        {
            ParameterName = nomeParametro,
            SqlDbType = estruturaCampo.TipoSql,
            Size = estruturaCampo.TamanhoMaximo,
            Value = valor,
            BaseConexao = baseConexao,
            EstruturaCampo = estruturaCampo
        };
    }

    [SetsRequiredMembers]
    public ParametroInfo(string nomeParametro, object? value)
        : this(nomeParametro, SqlUtil.GetBetterSqlDbType(value), value)
    {
        this.Size = SqlUtil.GetBetterSize(value);
    }

    [SetsRequiredMembers]
    public ParametroInfo(string nomeParametro,
                         SqlDbType sqlType,
                         object? value)
        : this(nomeParametro, sqlType, null, value)
    {

    }

    [SetsRequiredMembers]
    public ParametroInfo(string nomeParametro,
                         SqlDbType sqlType,
                         int? size,
                         object? value)
    {
        this.ParameterName = nomeParametro;
        this.Value = value;
        this.SqlDbType = sqlType;
        this.Size = size;
    }

    internal DbParameter GetDbParameter()
    {
        if (this.BaseConexao is not null &&
            this.EstruturaCampo is not null)
        {
            return this.BaseConexao.RetornarNovoParametro(this.EstruturaCampo,
                                                          this.ParameterName,
                                                          this.Value);
            //return this.BaseConexao.RetornarNovoParametro(this.ParameterName,
            //                                              this.SqlDbType.Value,
            //                                              this.Size,
            //                                              this.Value);
        }

        return new SqlParameter(this.ParameterName, this.SqlDbType)
        {
            Value = this.Value ?? DBNull.Value,
            Size = this.Size ?? 0,
            IsNullable = this.Value == null
        };
    }

    public override bool Equals(object? obj)
    {
        if (obj is ParametroInfo parametroInfo)
        {
            return this.ParameterName.Equals(parametroInfo.ParameterName, StringComparison.OrdinalIgnoreCase);
        }
        return base.Equals(obj);
    }

    public bool EqualsStrict(ParametroInfo other)
    {
        if (other is null)
        {
            return false;
        }

        if (Object.ReferenceEquals(this, other))
            return true;

        return this.ParameterName.Equals(other.ParameterName, StringComparison.OrdinalIgnoreCase)
            && this.SqlDbType == other.SqlDbType
            && this.Size == other.Size
            && Equals(this.Value, other.Value);
    }

    public override int GetHashCode()
    {
        return this.ParameterName.GetHashCode(StringComparison.OrdinalIgnoreCase);
    }

    public SqlParameter CreateSqlParameter()
    {
        return new SqlParameter(
            ParameterName,
            this.SqlDbType,
            this.Size ?? 0)
        {
            Value = this.Value ?? DBNull.Value
        };
    }
}