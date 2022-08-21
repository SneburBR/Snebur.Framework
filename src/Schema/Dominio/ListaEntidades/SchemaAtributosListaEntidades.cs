using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Snebur.Utilidade;
using Snebur.Dominio;

namespace Snebur.Schema
{
    public class SchemaAtributosListaEntidades : SchemaAtributosDominio
    {
    	public bool CarregarAutomaticoRodaPe { get; set; }
    
    	public int RegistrorPorPagina { get; set; }
    
    	public int LimiteRegistroPorPagina { get; set; }
    }
}