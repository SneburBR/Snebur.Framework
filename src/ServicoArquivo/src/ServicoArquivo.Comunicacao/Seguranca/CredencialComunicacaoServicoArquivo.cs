using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Seguranca;

namespace Snebur.ServicoArquivo
{
    public class CredencialComunicacaoServicoArquivo
    {
        public static CredencialServico ServicoArquivo
        {
            get
            {
                return new CredencialServico
                {
                    IdentificadorUsuario = CredencialComunicacaoServicoArquivo.IDENTIFICADOR_USUARIO,
                    Senha = CredencialComunicacaoServicoArquivo.SENHA
                };
            }
        }

        private const string IDENTIFICADOR_USUARIO = "ComunicacaoServicoArquivo";

        private const string SENHA = "7070b942-231b-4d46-aefb-1c176449e846";
    }
}
