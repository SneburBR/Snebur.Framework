using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Snebur.Utilidade;
using Snebur.Dominio;

namespace Snebur.Schema
{
    public class SchemaAtributosCampoInteiro : SchemaAtributosCampoNumero
    {
    
    	public int TotalDigitos { get; set; }
    
    	public int Minimo { get; set; }
    
    	public int Maximo { get; set; }
    }
}