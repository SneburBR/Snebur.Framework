namespace Snebur.Dominio.Atributos;

[IgnorarAtributoTS]
[AttributeUsage(AttributeTargets.Property)]
public class OrdenacaoOpcoesAttribute : Attribute
{
    public EnumOrdenacaoNovoRegistro OrdenacaoNovoRegistro { get; set; } = EnumOrdenacaoNovoRegistro.Fim;

    public bool IsIgnorarMigracao { get; set; }

    public string? NomePropriedadeMapeada { get; set; }

    public OrdenacaoOpcoesAttribute()
    {

    }

    [IgnorarConstrutorTS]
    public OrdenacaoOpcoesAttribute(EnumOrdenacaoNovoRegistro ordenacaoNovoRegistro)
    {
        this.OrdenacaoNovoRegistro = ordenacaoNovoRegistro;
    }
}
