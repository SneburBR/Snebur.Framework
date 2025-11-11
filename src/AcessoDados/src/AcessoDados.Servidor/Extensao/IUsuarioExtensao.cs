using System.Collections.Concurrent;

namespace Snebur;

public static class UsuarioExtensao
{
    public static readonly ConcurrentDictionary<Guid, object> _bloqueios = new ConcurrentDictionary<Guid, object>();

    public static object RetornarBloqueio(this IUsuario usuario)
    {
        var identificador = usuario.Identificador;
        return RetornarBloqueio(identificador);
    }

    private static object RetornarBloqueio(Guid identificador)
    {
        if (_bloqueios.TryGetValue(identificador, out var lockObj))
        {
            return lockObj;
        }
        var novoLockObj = new object();
        _bloqueios.TryAdd(identificador, novoLockObj);
        return novoLockObj;
    }
}