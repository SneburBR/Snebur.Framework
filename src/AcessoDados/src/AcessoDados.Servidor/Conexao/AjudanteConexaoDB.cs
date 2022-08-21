using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using System.Configuration;

namespace Snebur.AcessoDados
{
    internal class AjudanteConexaoDB
    {
        internal static BaseConexao RetornarConexao(BaseContextoDados contexto, string conectionString)
        {
            ConfiguracaoAcessoDados.TipoBancoDadosEnum = EnumTipoBancoDados.SQL_SERVER;
            return new ConexaoSqlServer(contexto, conectionString);

            //if (conectionString.ProviderName == "System.Data.SqlClient")
            //{
            //    ConfiguracaoAcessoDados.TipoBancoDadosEnum = EnumTipoBancoDados.SQL_SERVER;
            //    return new ConexaoSqlServer(contexto, conectionString);
            //}
            //return new ConexaoSqlServer(conectionString);
            //{

            //}
            //if ((Repositorio.TipoBancoDadosEnum == EnumTipoBancoDados.PostgreSQL) || (Repositorio.TipoBancoDadosEnum == EnumTipoBancoDados.PostgreSQLImob))
            //{

            //    return new ConexaoPostgreSql(conectionString);
            //}

            //if (Repositorio.TipoBancoDadosEnum == EnumTipoBancoDados.SQL_SERVER)
            //{
            //    return new ConexaoSqlServer(conectionString);
            //}

            throw new ErroNaoSuportado(String.Format("O tipo de banco de dados não é suportado {0} ", EnumUtil.RetornarDescricao(ConfiguracaoAcessoDados.TipoBancoDadosEnum)));
        }
    }
}