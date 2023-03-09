namespace Snebur.Seguranca
{
    public class CredencialAnonimo
    {

        public static CredencialUsuario Anonimo
        {
            get
            {
                return new CredencialUsuario
                {
                    IdentificadorUsuario = CredencialAnonimo.IDENTIFICADOR_USUARIO,
                    Senha = CredencialAnonimo.SENHA
                };
            }
        }

        private const string IDENTIFICADOR_USUARIO = "Anonimo";

        private const string SENHA = "c42b636c-97d1-4ea9-ba45-90ad98b42abb";

    }
    public class CredencialServicoAnonimo
    {
        public static CredencialServico Anonimo
        {
            get
            {
                return new CredencialServico
                {
                    IdentificadorUsuario = CredencialServicoAnonimo.IDENTIFICADOR_USUARIO,
                    Senha = CredencialServicoAnonimo.SENHA
                };
            }
        }

        private const string IDENTIFICADOR_USUARIO = "Anonimo";

        private const string SENHA = "c42b636c-97d1-4ea9-ba45-90ad98b42abb";

    }
}