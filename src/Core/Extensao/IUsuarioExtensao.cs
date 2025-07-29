using Snebur.Seguranca;

namespace Snebur;

public static class IUsuarioExtensao
{
    public static CredencialUsuario RetornarCredencial(this IUsuario usuario)
    {
        Guard.NotNull(usuario);
        return new CredencialUsuario
        {
            IdentificadorUsuario = usuario.IdentificadorUsuario,
            Senha = usuario.Senha,
            Nome = usuario.Nome
        };

    }
}