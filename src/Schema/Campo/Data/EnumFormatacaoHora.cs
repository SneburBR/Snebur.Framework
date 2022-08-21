using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Snebur.Utilidade;
using Snebur.Dominio;
using System.ComponentModel;

namespace Snebur.Schema
{
    
    public enum EnumFormatacaoHora
    {
    	[Description("Desconhecido")]
    	Desconhecido = 0,
    
    	[Description("HH:mm")]
    	HH_mm = 1,
    
    	[Description("HH:mm:ss")]
        HH_mm_ss = 2
    }
}