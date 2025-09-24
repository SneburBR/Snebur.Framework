namespace Snebur.Dominio.Atributos;

[IgnorarAtributoTS]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class IgnoreEntityModelMigrationAttribute : Attribute
{
    public string? Message { get; }
    public IgnoreEntityModelMigrationAttribute(string message)
    {
        this.Message = message;
    }
    public IgnoreEntityModelMigrationAttribute()
    {
    }
}
