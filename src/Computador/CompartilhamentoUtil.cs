using Snebur.Utilidade;
using System;
using System.Management;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Snebur.Computador
{
    public class CompartilhamentoUtil
    {
        public static void CompartilhaDiretorio(string caminhoDiretorio, string nomeCompartilhamento)
        {
            CompartilhamentoUtil.CompartilhaDiretorio(caminhoDiretorio, nomeCompartilhamento, null);
        }

        public static void CompartilhaDiretorio(string caminhoDiretorio, string nomeCompartilhamento, NTAccount conta)
        {
            DiretorioUtil.CriarDiretorio(caminhoDiretorio);
            try
            {
                var mc = new ManagementClass("win32_share");
                var mpc = mc.GetMethodParameters("Create");
                mpc["Path"] = caminhoDiretorio;
                mpc["Description"] = nomeCompartilhamento;
                mpc["Name"] = nomeCompartilhamento;
                mpc["Type"] = 0x0;
                mpc["MaximumAllowed"] = null;
                mpc["Password"] = null;
                mpc["Access"] = null;

                var outParams = mc.InvokeMethod("Create", mpc, null);
                if ((uint)(outParams.Properties["ReturnValue"].Value) != 0)
                {
                    //Ja existe o compartilhamento
                }

                if (conta != null)
                {
                    var userSID = (SecurityIdentifier)conta.Translate(typeof(SecurityIdentifier));
                    byte[] utenteSIDArray = new byte[userSID.BinaryLength];
                    userSID.GetBinaryForm(utenteSIDArray, 0);

                    var userTrustee = new ManagementClass(new ManagementPath("Win32_Trustee"), null);
                    userTrustee["Name"] = conta.Value;
                    userTrustee["SID"] = utenteSIDArray;

                    var userACE = new ManagementClass(new ManagementPath("Win32_Ace"), null);
                    userACE["AccessMask"] = 2032127;                                 //Full access
                    userACE["AceFlags"] = AceFlags.ObjectInherit | AceFlags.ContainerInherit;
                    userACE["AceType"] = AceType.AccessAllowed;
                    userACE["Trustee"] = userTrustee;

                    var userSecurityDescriptor = new ManagementClass(new ManagementPath("Win32_SecurityDescriptor"), null);
                    userSecurityDescriptor["ControlFlags"] = 4; //SE_DACL_PRESENT 
                    userSecurityDescriptor["DACL"] = new object[] { userACE };

                    //var mc = new ManagementClass("Win32_Share");
                    var share = new ManagementObject(mc.Path + ".Name='" + nomeCompartilhamento + "'");
                    share.InvokeMethod("SetShareInfo", new object[] { Int32.MaxValue, nomeCompartilhamento, userSecurityDescriptor });
                }
            }
            catch (Exception erro)
            {
                throw new Erro(String.Format("Não foi compartilhar o caminho {0}", caminhoDiretorio), erro);
            }
        }

    }
}

