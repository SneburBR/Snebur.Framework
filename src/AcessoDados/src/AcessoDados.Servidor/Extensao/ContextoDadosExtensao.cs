namespace Snebur.AcessoDados;
public static class ContextoDadosExtensao
{
    public static CacheSessaoUsuario GetRequiredCacheSessaoUsuario(this BaseContextoDados contextoDados)
    {
        return contextoDados.CacheSessaoUsuario
            ?? throw new Erro("O cache da sessão do usuário não foi inicializado");
    }

    public static IUsuario GetRequiredUsuarioLogado(this BaseContextoDados contextoDados)
    {
        return contextoDados.GetRequiredCacheSessaoUsuario().Usuario
            ?? throw new Erro("O usuário logado não foi inicializado");
    }

    public static ISessaoUsuario GetRequiredSessaoUsuarioLogado(this BaseContextoDados contextoDados)
    {
        return contextoDados.GetRequiredCacheSessaoUsuario().SessaoUsuario
            ?? throw new Erro("A sessão do usuário logado não foi inicializado");
    }
}
