using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Snebur.EntityFramework
{

    public class SqlUtils
    {

        public static string RetornarNomeTabela(Type tipo)
        {
            var atributoTabela = tipo.GetCustomAttributes<TableAttribute>().FirstOrDefault();
            if (atributoTabela == null)
            {
                throw new Exception("Tipo não possuio atributo Table");
            }
            else
            {
                return atributoTabela.Name;
            }
        }

        public static string RetornarNomeTabelaDBO(Type tipo)
        {
            var atributoTabela = tipo.GetCustomAttributes(true).OfType<TableAttribute>().FirstOrDefault();
            if (atributoTabela == null)
            {
                throw new Exception("Tipo não possuio atributo Table");
            }
            else
            {
                return string.Format("[dbo].[{0}]", atributoTabela.Name);
            }
        }

        public static string RetornarSqlDeletarTodosRegistro(Type tipo)
        {
            return string.Format(" DELETE FROM dbo.[{0}] ", SqlUtils.RetornarNomeTabela(tipo));
        }

        public static string RetornarSqlDeletarIndice(string nome, string tabela)
        {
            return string.Format(" IF EXISTS (select * from sys.indexes where object_id = OBJECT_ID(N'[dbo].[{0}]') AND name = N'{1}')    DROP INDEX [{1}] ON [dbo].[{0}] ", tabela, nome);
        }
    }
}