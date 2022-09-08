using Snebur.Utilidade;
using System;

namespace Snebur.AcessoDados
{
    internal class AjudanteSql
    {

        internal static string RetornarSqlFormatado(string sql, params object[] args)
        {
            var sqlFormatado = String.Format(sql, args);
            switch (ConfiguracaoAcessoDados.TipoBancoDadosEnum)
            {
                case EnumTipoBancoDados.SQL_SERVER:
                    //nao faz nada
                    break;
                case EnumTipoBancoDados.PostgreSQL:
                case EnumTipoBancoDados.PostgreSQLImob:

                    sqlFormatado = sqlFormatado.Replace("[", "\"").Replace("]", "\"");
                    break;
                default:
                    throw new NotSupportedException("Tipo de banco de dados não suportado " + EnumUtil.RetornarDescricao(ConfiguracaoAcessoDados.TipoBancoDadosEnum));
            }
            return sqlFormatado;
        }
    }
}