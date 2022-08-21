using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur.Dominio;

namespace Snebur.Comunicacao.Mensageiro
{
    public class Remetente : BaseDominio
    {

		#region Campos Privados


		#endregion


        IUsuario Usuario { get; set; }
    }
}