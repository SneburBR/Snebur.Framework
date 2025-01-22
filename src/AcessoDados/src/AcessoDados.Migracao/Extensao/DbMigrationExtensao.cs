using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Snebur
{
    public static class DbMigrationExtensao
    {
        public static bool ColumnExists(this DbMigration dbMigration,
                                        DbContext dbContext,
                                        string tableName,
                                        string columnName)
        {
            var sql = $@"SELECT COUNT(*)  FROM INFORMATION_SCHEMA.COLUMNS 
                            WHERE TABLE_NAME = '{tableName}' 
                            AND COLUMN_NAME = '{columnName}'";

            return dbContext.Database.SqlQuery<int>(sql).Any();
        }
    }
}
