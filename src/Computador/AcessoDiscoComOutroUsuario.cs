﻿using Snebur.IO;
using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Snebur.Computador
{
    public class AcessoDiscoComOutroUsuario : IDisposable
    {
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword,
       int dwLogonType, int dwLogonProvider, out SafeTokenHandle phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private extern static bool CloseHandle(IntPtr handle);

        public bool IsAutorizado = false;

        private WindowsIdentity WindowsIdentity { get; set; }
        private SafeTokenHandle SafeTokenHandle { get; set; }

#if NET6_0_OR_GREATER == false
        private WindowsImpersonationContext WindowsImpersonationContext { get; set; }
#endif

        public AcessoDiscoComOutroUsuario(string usuario, string senha) : this(null, usuario, senha)
        {

        }

        public AcessoDiscoComOutroUsuario(string dominio,
                                          string usuario,
                                          string senha) :
                                            this(new AcessoDiretorio(!String.IsNullOrEmpty(usuario),
                                                                     dominio,
                                                                     usuario,
                                                                     senha))
        {

        }

        public AcessoDiscoComOutroUsuario(IAcessoDiretorio acessoDiretorio)
        {
            if (acessoDiretorio.IsAutenticar)
            {
                var dominio = acessoDiretorio.Dominio;
                var usuario = acessoDiretorio.Usuario;
                var senha = acessoDiretorio.Senha;

                const int LOGON32_PROVIDER_DEFAULT = 0;
                const int LOGON32_LOGON_INTERACTIVE = 2;

                try
                {
                    bool returnValue = LogonUser(usuario,
                                                 dominio,
                                                 senha,
                                                 LOGON32_LOGON_INTERACTIVE,
                                                 LOGON32_PROVIDER_DEFAULT, out SafeTokenHandle safeTokenHandle);
                    if (returnValue && safeTokenHandle != null)
                    {
                        this.SafeTokenHandle = safeTokenHandle;
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            this.WindowsIdentity = new WindowsIdentity(safeTokenHandle.DangerousGetHandle());

#if NET6_0_OR_GREATER == false
                        this.WindowsImpersonationContext = this.WindowsIdentity.Impersonate();
#endif
                        }

                        this.IsAutorizado = true;
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        public void Dispose()
        {
#if NET6_0_OR_GREATER == false
            this.WindowsImpersonationContext?.Dispose();
#endif
            this.WindowsIdentity?.Dispose();
            this.SafeTokenHandle?.Dispose();
        }
    }
}
