namespace Snebur.Comunicacao;

public static class BaseComunicacaoServidorExtensao
{
    public static Guid GetRequiredIdentificadorSessao(this BaseComunicacaoServidor servidor)
    {
        if (servidor.IdentificadorSessaoUsuario is null)
        {
            throw new ErroNaoDefinido("Identificador da sessão do usuário não definido.");
        }

        if (servidor.IdentificadorSessaoUsuario == Guid.Empty)
            throw new ErroNaoDefinido("Identificador da sessão do usuário não pode ser Guid.Empty.");

        return servidor.IdentificadorSessaoUsuario.Value;
    }

    public static CredencialUsuario GetRequiredCredencialUsuario(this BaseComunicacaoServidor servidor)
    {
        if (servidor.CredencialUsuario is null)
        {
            throw new ErroNaoDefinido($"A credencial do usuário não foi definidA. {servidor.GetType().Name}");
        }
        return servidor.CredencialUsuario;
    }

    public static string GetRequiredIdentificadorProprietario(this BaseComunicacaoServidor servidor)
    {
        if (string.IsNullOrWhiteSpace(servidor.IdentificadorProprietario))
        {
            throw new ErroNaoDefinido($"Identificador do proprietário não definido. {servidor.GetType().Name}");
        }
        return servidor.IdentificadorProprietario;
    }

    public static Guid GetRequiredIdentificadorSessaoUsuario(this BaseComunicacaoServidor servidor)
    {
        if (servidor.IdentificadorSessaoUsuario is null)
        {
            throw new ErroNaoDefinido($"Identificador da sessão do usuário não definido. {servidor.GetType().Name}");
        }
        if (servidor.IdentificadorSessaoUsuario == Guid.Empty)
            throw new ErroNaoDefinido($"Identificador da sessão do usuário não pode ser Guid.Empty. {servidor.GetType().Name}");

        return servidor.IdentificadorSessaoUsuario.Value;
    }
}
