using Snebur.Seguranca;

namespace Snebur.ServicoArquivo.Seguranca;

public class CredencialServicoArquivo
{
    public static Credencial ServicoArquivo
    {
        get
        {
            return new CredencialServico
            {
                IdentificadorUsuario = IDENTIFICADOR_USUARIO,
                Senha = SENHA
            };
        }
    }

    private const string IDENTIFICADOR_USUARIO = "ServicoArquivo";

    private const string SENHA = "0388cb06-7431-4c5e-b70f-31129e7fd7da";
}
