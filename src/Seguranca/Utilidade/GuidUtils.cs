using System.Text.RegularExpressions;

namespace Snebur.Utilidade;


public class GuidUtil
{

    private static Regex guidRegex = new Regex("^(\\{){0,1}[0-9a-fA-F]{8}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{12}(\\}){0,1}$", RegexOptions.Compiled);

    public static bool GuidValido(string conteudo)
    {
        return Guid.TryParse(conteudo, out Guid guid) && guid != Guid.Empty;
        //return conteudo != null && guidRegex.IsMatch(conteudo) && new Guid(conteudo) != Guid.Empty;
    }

    public static bool GuidValido(Guid? guid)
    {
        return guid.GetValueOrDefault() != Guid.Empty;
        //return conteudo != null && guidRegex.IsMatch(conteudo) && new Guid(conteudo) != Guid.Empty;
    }
    public static bool GuidValido(Guid guid)
    {
        return guid != Guid.Empty;
        //return conteudo != null && guidRegex.IsMatch(conteudo) && new Guid(conteudo) != Guid.Empty;
    }

    public static Guid RetonrarGuid(string value)
    {
        return new Guid(Md5Util.RetornarHash(value));
    }

}
