using NetFwTypeLib;
using System;

namespace Snebur.Computador
{
    public class FireWallUtil
    {
        public static INetFwMgr WinFirewallManager()
        {
            Type type = Type.GetTypeFromCLSID(new Guid("{98147eeb-637d-49eb-b196-9cf963bf2716}"));
            return Activator.CreateInstance(type) as INetFwMgr;
        }

        public static bool AbrirPortaFireWall()
        {
            //Não implementado
            var caminhoAplicacao = AplicacaoSnebur.Atual.CaminhoExecutavel;
            var tipoAutorizacao = Type.GetTypeFromProgID("HNetCfg.FwAuthorizedApplication");
            var autorizacaoFireWall = Activator.CreateInstance(tipoAutorizacao) as INetFwAuthorizedApplication;
            autorizacaoFireWall.Name = AplicacaoSnebur.Atual.NomeAplicacao;
            autorizacaoFireWall.ProcessImageFileName = caminhoAplicacao;
            autorizacaoFireWall.Scope = NET_FW_SCOPE_.NET_FW_SCOPE_ALL;
            //autorizacaoFireWall.IpVersion = NET_FW_IP_VERSION_.NET_FW_IP_VERSION_V4;
            autorizacaoFireWall.Enabled = true;

            INetFwMgr mgr = WinFirewallManager();
            try
            {
                mgr.LocalPolicy.CurrentProfile.AuthorizedApplications.Add(autorizacaoFireWall);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.Write(ex.Message);
                return false;
            }
            return true;
        }
    }
}


