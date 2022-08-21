using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using Snebur.Seguranca;

namespace Snebur.AcessoDados
{
    public partial class CacheSessaoUsuario : IDisposable
    {
        private partial class AjudanteSessaoUsuarioInterno
        {
            private static object Bloqueio { get; set; } = new object();
            private static AjudanteSessaoUsuarioInterno AjudanteSessaoUsuario { get; set; }

            internal static AjudanteSessaoUsuarioInterno RetornarAjudanteUsuario(BaseContextoDados contexto)
            {
                if (AjudanteSessaoUsuario == null)
                {
                    lock (Bloqueio)
                    {
                        if (AjudanteSessaoUsuario == null)
                        {
                            AjudanteSessaoUsuario = new AjudanteSessaoUsuarioInterno(contexto, contexto.RetornarUsuariosSistemaInterno());
                        }
                    }
                }
                AjudanteSessaoUsuario.Contexto = contexto;
                return AjudanteSessaoUsuario;
            }


        }

        public static void FinalizarSessaoUsuario(Guid identificadorSessaoUsuario)
        {
            var cacheSesssao = RetornarCacheSessaoUsuario(identificadorSessaoUsuario, true);
            cacheSesssao?.FinalizarSessaoUsuario();
        }
    }
}