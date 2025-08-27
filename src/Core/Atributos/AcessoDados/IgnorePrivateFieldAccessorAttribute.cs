namespace Snebur.Dominio.Atributos;

[AttributeUsage(AttributeTargets.Property)]
public class IgnorePrivateFieldAccessorAttribute : Attribute
{
    public string? Message { get; }
    public IgnorePrivateFieldAccessorAttribute(string? message = null)
    {
        this.Message = message;
    }

    public IgnorePrivateFieldAccessorAttribute()
    {
    }
}