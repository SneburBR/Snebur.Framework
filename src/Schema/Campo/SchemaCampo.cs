using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Snebur.Utilidade;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;

namespace Snebur.Schema
{
    
    public abstract class SchemaCampo : SchemaConteudo
    {
    	[IgnorarPropriedadeTS()]
    	public ValueCampo value { get; set; }
    }
}