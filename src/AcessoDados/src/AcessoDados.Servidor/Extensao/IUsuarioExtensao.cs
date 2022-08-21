using System;
using System.Collections.Generic;
using Snebur.Dominio;

namespace Snebur
{
    public static class UsuarioExtensao
    {
        public static readonly Dictionary<Guid, object> _bloqueios = new Dictionary<Guid, object>();
        private readonly static object _bloqueio = new object();

        public static object RetornarBloqueio(this IUsuario usuario)
        {
            var identificador = usuario.Identificador;
            return RetornarBloqueio(identificador);
        }

        private static object RetornarBloqueio(Guid identificador)
        {
            if (!_bloqueios.ContainsKey(identificador))
            {
                lock (_bloqueio)
                {
                    if (!_bloqueios.ContainsKey(identificador))
                    {
                        _bloqueios.Add(identificador, new object());
                    }
                }
            }
            return _bloqueios[identificador];
        }
    }
}