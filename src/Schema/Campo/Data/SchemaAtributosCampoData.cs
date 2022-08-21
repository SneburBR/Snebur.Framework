using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Snebur.Utilidade;
using Snebur.Dominio;

namespace Snebur.Schema
{
    
    public class SchemaAtributosCampoData : SchemaAtributosCampo
    {
    
    	public bool MostrarData { get; set; }
    
    	public bool MostrarHora { get; set; }
    
    	public EnumFormatacaoData FormatacaoData { get; set; }
    
    	public EnumFormatacaoHora EnumFormatacaoHora { get; set; }
    }
}