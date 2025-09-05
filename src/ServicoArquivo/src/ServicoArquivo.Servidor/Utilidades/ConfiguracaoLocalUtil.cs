namespace Snebur.ServicoArquivo.Servidor;

public static class ConfiguracaoLocalUtil
{
    private const string CHAVE_AUTENTICAR_ACESSO_COMPARTILHADO = "IsAutenticarAcessoCompartilhado";
    private const string CHAVE_NOME_COMPUTADOR = "NomeComputadorAcesso";
    private const string CHAVE_USUARIO = "Usuario";
    private const string CHAVE_SENHA = "Senha";
    public static bool IsAutenticarAcessoCompartilhado
        => Convert.ToBoolean(ConfiguracaoUtil.AppSettings?[CHAVE_AUTENTICAR_ACESSO_COMPARTILHADO]);
    public static string NomeComputadorAcesso
        => field ??= ConfiguracaoUtil.AppSettings?[CHAVE_NOME_COMPUTADOR]
            ?? throw new InvalidOperationException($"A chave '{CHAVE_NOME_COMPUTADOR}' não foi encontrada no arquivo de configuração.");

    public static string Usuario
        => field ??= ConfiguracaoUtil.AppSettings?[CHAVE_USUARIO]
            ?? throw new InvalidOperationException($"A chave '{CHAVE_USUARIO}' não foi encontrada no arquivo de configuração.");
    public static string Senha
        => field ??= ConfiguracaoUtil.AppSettings?[CHAVE_SENHA]
            ?? throw new InvalidOperationException($"A chave '{CHAVE_SENHA}' não foi encontrada no arquivo de configuração.");
}
