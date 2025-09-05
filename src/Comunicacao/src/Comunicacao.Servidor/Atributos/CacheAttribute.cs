using Snebur.Dominio.Atributos;

namespace Snebur.Comunicacao;

[IgnorarAtributoTS]
[AttributeUsage(AttributeTargets.Method)]
public class CacheAttribute : Attribute
{
    public int ExpirarCacheEmMinutos { get; set; }

    public CacheAttribute(int expirarCacheEmMinutos = 0)
    {
        this.ExpirarCacheEmMinutos = expirarCacheEmMinutos;
    }
}
