namespace Snebur.Seguranca
{
    public class CredencialAtualizacao
    {

        public static Credencial Atualizacao
        {
            get
            {
                return new CredencialServico
                {
                    IdentificadorUsuario = CredencialAtualizacao.IDENTIFICADOR_USUARIO,
                    Senha = CredencialAtualizacao.SENHA
                };
            }
        }

        private const string IDENTIFICADOR_USUARIO = "Atualizacao";

        private const string SENHA = "bf73ae7d-552f-4fe4-88bc-5d4e2b9abf34";
    }
}
