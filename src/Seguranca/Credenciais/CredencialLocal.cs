namespace Snebur.Seguranca
{
    public class CredencialLocal
    {

        public static CredencialServico Local
        {
            get
            {
                return new CredencialServico
                {
                    IdentificadorUsuario = CredencialLocal.IDENTIFICADOR_USUARIO,
                    Senha = CredencialLocal.SENHA
                };
            }
        }

        private const string IDENTIFICADOR_USUARIO = "Local";

        private const string SENHA = "dda8c9d8-e7cd-45ad-a4fe-aa667aedca60";
    }
}
