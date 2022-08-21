using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Snebur.Utilidade;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;

namespace Snebur.Schema
{
    public class SchemaListaEntidades : SchemaDominio
    {
    	//Implements ICaminhosAtributos
    
    	//Public Property CaminhoHTML As Uri Implements ICaminhosAtributos.CaminhoHTML
    
    	//Public Property CaminhoXML As Uri Implements ICaminhosAtributos.CaminhoXML
    
    	//<SchemaPropridadeElemento>
    	[IgnorarPropriedadeTS()]
    	public SchemaConteudo Cabecalho { get; set; }
    
    	//<SchemaPropridadeElemento>
    	[IgnorarPropriedadeTS()]
    	public SchemaConteudo Separador { get; set; }
    
    	//<SchemaPropridadeElemento>
    	[IgnorarPropriedadeTS()]
    	public SchemaConteudo NenhumRegistroEncontrado { get; set; }
    
    	//<SchemaPropridadeElemento>
    	[IgnorarPropriedadeTS()]
    	public SchemaConteudo RodaPe { get; set; }
    
    	//<IgnorarPropriedade>
    	//Property Normal As EntidadeEstrutura
    
    	//<IgnorarPropriedade>
    	//Property Selecionado As EntidadeEstrutura
    
    	//<IgnorarPropriedade>
    	//Property Alternado As EntidadeEstrutura
    }
}