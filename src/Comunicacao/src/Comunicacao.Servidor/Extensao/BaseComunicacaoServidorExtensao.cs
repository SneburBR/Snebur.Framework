namespace Snebur.Comunicacao;

public static class BaseComunicacaoServidorExtensao
{
    public static Guid GetIdentificadorSessaoRequired(this BaseComunicacaoServidor servidor)
    {
        if (servidor.IdentificadorSessaoUsuario is null)
        {
            throw new ErroNaoDefinido("Identificador da sessão do usuário não definido.");
        }
        if (servidor.IdentificadorSessaoUsuario == Guid.Empty)
            throw new ErroNaoDefinido("Identificador da sessão do usuário não pode ser Guid.Empty.");

        return servidor.IdentificadorSessaoUsuario.Value;
    }
}
