﻿using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Snebur.Computador
{

    /// <summary>
    /// Acesso rede o disco local um autenticação do usuario existente no computador local
    /// </summary>
    public class AcessoDiscoRedeAutenticadoUtil
    {
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
        int dwLogonType, int dwLogonProvider, out SafeTokenHandle phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private extern static bool CloseHandle(IntPtr handle);

        public static void Acessar(string usuario, string senha, Action callback)
        {
            AcessoDiscoRedeAutenticadoUtil.Acessar(null, usuario, senha, callback, null);
        }

        public static void Acessar(string usuario, string senha, Action callback, Action<Exception> callbackErro)
        {
            AcessoDiscoRedeAutenticadoUtil.Acessar(null, usuario, senha, callback, callbackErro);
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void Acessar(string dominio, string usuario, string senha, Action callbackSucesso, Action<Exception> callbackErro)
        {
            SafeTokenHandle safeTokenHandle;
            try
            {

                const int LOGON32_PROVIDER_DEFAULT = 0;
                const int LOGON32_LOGON_INTERACTIVE = 2;

                bool returnValue = LogonUser(usuario, dominio, senha, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, out safeTokenHandle);

                if (returnValue == false)
                {
                    int retorno = Marshal.GetLastWin32Error();
                    throw new System.ComponentModel.Win32Exception(retorno);
                }

                using (safeTokenHandle)
                {
                    using (WindowsIdentity newId = new WindowsIdentity(safeTokenHandle.DangerousGetHandle()))
                    {
                        using (WindowsImpersonationContext impersonatedUser = newId.Impersonate())
                        {
                            callbackSucesso.Invoke();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                callbackErro?.Invoke(ex);
            }
        }
    }

    public sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeTokenHandle()
            : base(true)
        {
        }

        [DllImport("kernel32.dll")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr handle);

        protected override bool ReleaseHandle()
        {
            return CloseHandle(handle);
        }
    }


}
