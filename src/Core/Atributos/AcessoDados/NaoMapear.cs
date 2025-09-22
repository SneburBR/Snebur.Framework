using System.ComponentModel.DataAnnotations.Schema;

namespace Snebur.Dominio.Atributos;

[IgnorarAtributoTS]
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
public class NaoMapearAttribute : NotMappedAttribute
{
}
