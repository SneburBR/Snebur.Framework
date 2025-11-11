using Snebur.Seguranca;
namespace Snebur.Dominio;

public interface ICredencial
{
    [ValidacaoRequerido]
    [ValidacaoUnico]
    [ValidacaoTextoTamanho(100)]
    string? IdentificadorUsuario { get; }

    [PropriedadeOpcionalTS]
    string? IdentificadorAmigavel { get; }

    [ValidacaoSenha]
    [ValidacaoTextoTamanho(36)]
    string? Senha { get; }
}
 
public static class CredencialExtensions
{

    public static bool IsValid(this ICredencial credencial)
    {
        return !string.IsNullOrWhiteSpace(credencial?.IdentificadorUsuario) &&
               !string.IsNullOrWhiteSpace(credencial?.Senha);
    }

    public static bool IsAnonimo(this ICredencial credencial)
    {
        return credencial.IsValid() &&
            CredencialUtil.ValidarCredencial(credencial, CredencialAnonimo.Anonimo);
    }

    public static bool Validar(this ICredencial credencial1, ICredencial credencial2)
    {
        return CredencialUtil.ValidarCredencial(credencial1, credencial2);
    }
}
