namespace Snebur.Dominio.Atributos;

[AttributeUsage(AttributeTargets.Field)]
public class EnumTSStringAttribute : BaseAtributoDominio
{
    public string TSValue { get; }
    public EnumTSStringAttribute(string tsValue)
    {
        this.TSValue = tsValue;
    }
}

