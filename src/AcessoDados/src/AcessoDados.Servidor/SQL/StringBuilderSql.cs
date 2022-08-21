using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;

namespace Snebur.AcessoDados
{
    internal class StringBuilderSql
    {

        private StringBuilder SB { get; set; }

        internal StringBuilderSql()
        {
            this.SB = new StringBuilder();
        }

        internal void Append(string sql)
        {
            this.SB.Append(AjudanteSql.RetornarSqlFormatado(sql));
        }

        internal void AppendLine(string sql)
        {
#if DEBUG
            this.SB.AppendLine(AjudanteSql.RetornarSqlFormatado(sql));
#else
            this.SB.Append(" ");
            this.SB.Append(AjudanteSql.RetornarSqlFormatado(sql));
#endif
        }

        internal void AppendLineFormat(string sql, params object[] args)
        {
            this.SB.AppendLine(AjudanteSql.RetornarSqlFormatado(sql, args));
        }

        internal void AppendFormat(string sql, params object[] args)
        {
            this.Append(AjudanteSql.RetornarSqlFormatado(sql, args));
        }

        public override string ToString()
        {
            return this.SB.ToString();
        }
    }
}