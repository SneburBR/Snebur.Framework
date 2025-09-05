using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace Snebur.Computador;

public class UsuarioUtil
{

    public static void CriarUsuario(string nomeUsuario, string senha)
    {
        if (!UsuarioUtil.ExisteUsuario(nomeUsuario))
        {
            using (var computadorLocal = new DirectoryEntry("WinNT://.,computer"))
            {
                using (var usuario = computadorLocal.Children.Add(nomeUsuario, "user"))
                {
                    usuario.Properties["FullName"].Value = nomeUsuario;
                    usuario.Invoke("SetPassword", new Object[] { senha });

                    //senha nunca expira
                    usuario.Invoke("Put", new Object[] { "UserFlags", 0x10000 });

                    usuario.CommitChanges();
                    usuario.Close();
                }
            }
        }
    }

    public static bool ExisteUsuario(string nomeUsuario)
    {
        using (var pc = new PrincipalContext(ContextType.Machine))
        {
            var usuario = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, nomeUsuario);
            return usuario != null;
        }
    }
}
