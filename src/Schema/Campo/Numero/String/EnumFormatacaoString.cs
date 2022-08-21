using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Snebur.Utilidade;
using Snebur.Dominio;
using System.ComponentModel;

namespace Snebur.Schema
{
    public enum EnumFormatacaoString
    {
    	[Description("Desconhecido")]
    	Desconhecido = 0,
    
    	[Description("Normal")]
    	Normal = 1,
    
    	[Description("Maiuscula")]
    	Maiuscula = 2,
    
    	[Description("Minuscula")]
    	Minuscula = 3
    }
}