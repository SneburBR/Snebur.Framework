namespace Snebur.Dominio.Atributos;

[IgnorarAtributoTS]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class IgnoreAlterTableMigrationAttribute : Attribute
{
    public string? Message { get; }
    public IgnoreAlterTableMigrationAttribute(string message)
    {
        this.Message = message;
    }

    public IgnoreAlterTableMigrationAttribute()
    {
    }
}
