using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Snebur.Dominio;

namespace Snebur.Seguranca
{
    public class CredencialServicoGlobalizacao
    {
        public static CredencialServico Globalizacao
        {
            get
            {
                return new CredencialServico
                {
                    IdentificadorUsuario = CredencialServicoGlobalizacao.IDENTIFICADOR_USUARIO,
                    Senha = CredencialServicoGlobalizacao.SENHA
                };
            }
        }

        private const string IDENTIFICADOR_USUARIO = "ServicoGlobalizacao";
    
    	private const string SENHA = "fba8079f-e3b9-437f-ba6d-17a517dfa265";
    }
}
