namespace Snebur.Seguranca;

public class CredencialUsuarioComunicacaoServicoArquivo
{
    public static CredencialUsuario UsuarioComunicacaoServicoArquivo
    {
        get
        {
            return new CredencialUsuario
            {
                IdentificadorUsuario = CredencialUsuarioComunicacaoServicoArquivo.IDENTIFICADOR_USUARIO,
                Senha = CredencialUsuarioComunicacaoServicoArquivo.SENHA
            };
        }
    }

    private const string IDENTIFICADOR_USUARIO = "UsuarioComunicacaoServicoArquivo";

    private const string SENHA = "1ecf0af6-3db6-44ad-93f0-e05b4a05e9f5";
}
