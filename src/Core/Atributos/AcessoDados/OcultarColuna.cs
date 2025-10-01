using System.ComponentModel.DataAnnotations;

namespace Snebur.Dominio.Atributos;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class OcultarColunaAttribute : ScaffoldColumnAttribute, IDomainAtributo
{
    public OcultarColunaAttribute() : base(false)
    {
    }
}