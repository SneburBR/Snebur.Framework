using System.ComponentModel.DataAnnotations.Schema;

namespace Snebur.Dominio.Atributos;

[IgnorarAtributoTS]
[AttributeUsage(AttributeTargets.Property)]
public class CampoAttribute : ColumnAttribute
{
    //public CampoAttribute()
    //{

    //}
    public CampoAttribute(string columnName) : base(columnName)
    {
        
    }
}

[IgnorarAtributoTS]
[AttributeUsage(AttributeTargets.Property)]
public class TipoBancoAttribute : ColumnAttribute
{
    public Type Tipo { get; }

    public TipoBancoAttribute(Type tipo)
    {
        this.Tipo = tipo;
    }
}