using System.Data;

namespace Snebur.Dominio.Atributos;

public class SqlDbTypeAttribute : Attribute
{
    public SqlDbType SqlDbType { get; }
    public SqlDbTypeAttribute( SqlDbType sqlDbType)
    {
        this.SqlDbType = sqlDbType;
    }
}