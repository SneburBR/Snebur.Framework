using Snebur.Dominio.Atributos;

namespace Snebur.Comunicacao;

[IgnorarAtributoTS]
[AttributeUsage(AttributeTargets.Method)]
public class IgnorarCacheAttribute : Attribute
{
}
