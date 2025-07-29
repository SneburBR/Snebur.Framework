namespace Snebur.Dominio
{
    public enum EnumStatusSessaoUsuario
    {

        Desconhecido = 0,
        Nova = 100,

        Ativo = 200,

        Inativo = 300,

        Finalizada = 400,

        Bloqueado = 500,

        Expirado = 600,

        Cancelada = 700,

        SenhaAlterada = 800,

        UsuarioDiferente = 900,

        IdentificadorSessaoUsuarioInexistente = 1000,

    }
}