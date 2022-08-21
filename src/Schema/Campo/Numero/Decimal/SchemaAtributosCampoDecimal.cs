using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Snebur.Utilidade;
using Snebur.Dominio;

namespace Snebur.Schema
{
    public class SchemaAtributosCampoDecimal : SchemaAtributosCampoNumero
    {
    
    	public int Minimo { get; set; }
    
    	public int Maximo { get; set; }
    
    	public int DigitosDecimal { get; set; }
    
    	public bool Arredondar { get; set; }
    }
}