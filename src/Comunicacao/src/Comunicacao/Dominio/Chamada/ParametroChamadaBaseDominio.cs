using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Snebur.Dominio.Atributos;
using Snebur.Dominio;

namespace Snebur.Comunicacao
{
    public class ParametroChamadaBaseDominio : ParametroChamada
    {
		#region Campos Privados

		#endregion

    	public BaseDominio BaseDominio { get; set; }
    }
}