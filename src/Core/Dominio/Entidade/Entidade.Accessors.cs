using System.Runtime.CompilerServices;

namespace Snebur.Dominio;

public abstract partial class Entidade
{
    internal virtual TValue GetPropertyValue<TValue>(
        ref TValue oldValue, [CallerMemberName] string nomePropriedade = "")
        where TValue : notnull
    {
        throw new NotImplementedException();
    }

     
}