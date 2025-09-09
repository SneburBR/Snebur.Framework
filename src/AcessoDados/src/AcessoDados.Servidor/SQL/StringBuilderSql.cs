using System.Text;

namespace Snebur.AcessoDados;

internal class StringBuilderSql
{
    private StringBuilder sb { get; set; }

    internal StringBuilderSql()
    {
        this.sb = new StringBuilder();
    }

    internal void Append(string sql)
    {
        this.sb.Append(this.Normalizar(sql));
    }

    internal void AppendLine(string sql)
    {
        this.sb.AppendLine(this.Normalizar(sql));
    }

    internal void AppendLineFormat(string sql, params object[] args)
    {
        this.sb.AppendLine(AjudanteSql.RetornarSqlFormatado(sql, args));
    }

    internal void AppendFormat(string sql, params object[] args)
    {
        this.Append(AjudanteSql.RetornarSqlFormatado(sql, args));
    }

    internal string Normalizar(string sql)
    {
        return sql;

        //switch (ConfiguracaoAcessoDados.TipoBancoDadosEnum)
        //{
        //    case EnumTipoBancoDados.SQL_SERVER:
        //        //nao faz nada
        //        return sql;

        //    case EnumTipoBancoDados.PostgreSQL:
        //    case EnumTipoBancoDados.PostgreSQLImob:

        //        return sql.Replace("[", "\"").Replace("]", "\"");

        //    default:
        //        throw new NotSupportedException("Tipo de banco de dados não suportado " + EnumUtil.RetornarDescricao(ConfiguracaoAcessoDados.TipoBancoDadosEnum));
        //}

    }

    public override string ToString()
    {
        return this.sb.ToString();
    }
}