namespace Snebur.Dominio.Atributos;

[IgnorarAtributoTS]
[AttributeUsage(AttributeTargets.Property)]
public class IgnoreNavigationPropertyAttribute : Attribute
{
    public string? Message { get; }
    public IgnoreNavigationPropertyAttribute(string message)
    {
        this.Message = message;
    }

    public IgnoreNavigationPropertyAttribute()
    {
    }
}
