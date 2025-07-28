using Snebur.Dominio;
using Snebur.Seguranca;
using Snebur.Utilidade;

namespace Snebur
{
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
}