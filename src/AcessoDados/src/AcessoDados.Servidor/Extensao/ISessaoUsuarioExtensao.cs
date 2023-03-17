using Snebur.Dominio;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Snebur
{
    public static class SessaoUsuarioExtensao
    {
        public static readonly Dictionary<Guid, object> _bloqueios = new Dictionary<Guid, object>();
        private readonly static object _bloqueio = new object();

        public static object RetornarBloqueio(this ISessaoUsuario sessaoUsuario)
        {
            var identificador = sessaoUsuario.IdentificadorSessaoUsuario;
            return RetornarBloqueio(identificador);
        }

        internal static object RetornarBloqueio(Guid identificador)
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
