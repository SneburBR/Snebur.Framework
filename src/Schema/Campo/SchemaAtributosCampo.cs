using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Snebur.Utilidade;
using Snebur.Dominio;

namespace Snebur.Schema
{
    public abstract class SchemaAtributosCampo : SchemaAtributosConteudo
    {
    
    	public bool Requerido { get; set; }
    
    	public string ValorPadrao { get; set; }
    
    	public string Valor { get; set; }
    }
}