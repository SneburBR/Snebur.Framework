namespace Snebur.Seguranca
{
    public class CredencialServicoArquivo
    {

        public static CredencialServico ServicoArquivo
        {
            get
            {
                return new CredencialServico
                {
                    IdentificadorUsuario = CredencialServicoArquivo.IDENTIFICADOR_USUARIO,
                    Senha = CredencialServicoArquivo.SENHA
                };
            }
        }

        private const string IDENTIFICADOR_USUARIO = "ServicoArquivo";

        private const string SENHA = "3e74aef0-0177-4ef7-a6cb-fa9d23a58b85";
    }



}
