namespace Snebur.Dominio.Atributos;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
[IgnorarAtributoTS]
public class TipoTSAttribute : Attribute
{
    //public string? CaminhoTipoTS { get; set; }

    public Type TsType { get; }

    public TipoTSAttribute(Type type)
    {
        TsType = type;
    }
    //public TipoTSAttribute(string caminhoTipoTS)
    //{
    //    Guard.NotNullOrWhiteSpace(caminhoTipoTS);
    //    this.CaminhoTipoTS = caminhoTipoTS;
    //}
}