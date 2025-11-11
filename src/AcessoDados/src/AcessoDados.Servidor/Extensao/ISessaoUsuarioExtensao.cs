using System.Collections.Concurrent;

namespace Snebur;

public static class SessaoUsuarioExtensao
{
    public static readonly ConcurrentDictionary<Guid, object> _bloqueios = new ConcurrentDictionary<Guid, object>();
    public static object RetornarBloqueio(this ISessaoUsuario sessaoUsuario)
    {
        var identificador = sessaoUsuario.IdentificadorSessaoUsuario;
        return RetornarBloqueio(identificador);
    }

    internal static object RetornarBloqueio(Guid identificador)
    {
        if (_bloqueios.TryGetValue(identificador, out var obj))
        {
            return obj;
        }
        var lockObj = new object();
        _bloqueios.TryAdd(identificador, lockObj);
        return lockObj;
    }
}
