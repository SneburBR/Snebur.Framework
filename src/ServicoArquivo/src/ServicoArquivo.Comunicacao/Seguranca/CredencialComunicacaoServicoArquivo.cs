/* Unmerged change from project 'Snebur.ServicoArquivo.Servidor'
Before:
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
After:
using Snebur;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Seguranca;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Seguranca;
*/

/* Unmerged change from project 'Snebur.ServicoArquivo.Comunicacao'
Before:
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
After:
using Snebur;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Seguranca;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Seguranca;
*/

namespace Snebur.ServicoArquivo;

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
