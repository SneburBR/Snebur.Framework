
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
                return ValidarSenha(credencial1.Senha, credencial2.Senha);
            }
        }
        return false;

    }

    public static bool ValidarSenha(string? senha1, string? senha2)
    {
        if (String.IsNullOrWhiteSpace(senha1) || String.IsNullOrWhiteSpace(senha2))
        {
            return false;
        }

        if (senha1 == senha2)
        {
            return true;
        }

        var senha1Md5 = NormalizarSenhaMd5(senha1);
        var senha2Md5 = NormalizarSenhaMd5(senha2);
        return senha1Md5.Equals(senha2Md5, StringComparison.OrdinalIgnoreCase);
    }

    public static string NormalizarSenhaMd5(string senha)
    {
        if (Md5Util.IsMd5(senha))
        {
            return senha;
        }
        return Md5Util.RetornarHash(senha);
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