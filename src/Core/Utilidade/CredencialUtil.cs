
namespace Snebur.Utilidade;

public class CredencialUtil
{
    public static bool ValidarCredencial(
        ICredencial? credencial1,
        ICredencial? credencial2)
    {
        if (credencial1?.IsValid() == true && credencial2?.IsValid() == true)
        {
            if (CredencialUtil.ValidarIdentificador(credencial1, credencial2))
            {
                if (credencial1.Senha == credencial2.Senha)
                {
                    return true;
                }

                var senha1 = credencial1.Senha!.Trim().ToLower();
                var senha2 = credencial2.Senha!.Trim().ToLower();
                var senha1Md5 = Md5Util.RetornarHash(senha1);
                var senha2Md5 = Md5Util.RetornarHash(senha2);

                return senha1 == senha2 ||
                       senha1 == senha2Md5 ||
                       senha1Md5 == senha2 ||
                       senha1Md5 == senha2Md5;
            }
        }
        return false;

    }

    private static bool ValidarIdentificador(ICredencial credencial1, ICredencial credencial2)
    {
        var identificador1 = CleanUp(credencial1.IdentificadorUsuario);
        var identificador2 = CleanUp(credencial2.IdentificadorUsuario);

        var amigavel1 = credencial1.IdentificadorAmigavel;
        var amigavel2 = credencial2.IdentificadorAmigavel;

        return EqualsInternal(identificador1, identificador2) ||
               EqualsInternal(identificador1, amigavel2) ||
               EqualsInternal(amigavel1, identificador1) ||
               EqualsInternal(amigavel1, amigavel2);
    }

    private static string? CleanUp(string? identificadorUsuario)
    {
        if (identificadorUsuario?.Contains('|') == true)
        {
            return identificadorUsuario.Substring(0, identificadorUsuario.IndexOf('|'));
        }
        return identificadorUsuario;
    }

    private static bool EqualsInternal(string? str1, string? str2)
    {
        if (str1 is null || str2 is null)
        {
            return false;
        }
        return str1.Trim().Equals(str2.Trim(), StringComparison.OrdinalIgnoreCase);
    }
    //public static Credencial RetornarCredencial(string caminhoArquivo)
    //{
    //    var conteudo = File.ReadAllText(caminhoArquivo, Encoding.UTF8);
    //    var json = CriptografiaUtil.Descriptografar(CHAVE_CRIPTOGRAFIA, conteudo);
    //    return JsonUtil.Deserializar<CredencialUsuario>(json);
    //}

    //public static Credencial SalvarCredencial(string identificador, Credencial credencial)
    //{

    //}
}