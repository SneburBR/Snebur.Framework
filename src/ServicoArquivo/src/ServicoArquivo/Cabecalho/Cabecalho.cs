//using System;
//using System.Linq;
//using System.Text;
//using System.Collections.Generic;
//using System.Web;

//namespace Snebur.Seguranca
//{
    
//    public class Cabecalho : BaseCabecalho
//    {
    
//    	public long IdCliente { get; set; }
//    	public Guid IdentificadorUsuario { get; set; }
    
//    	public Cabecalho(HttpContext context) :base(context)
//    	{
//            Guid identificadorUsuario;

//    		if (Guid.TryParse(this.RetornarString(ConstantesCabecalho.IDENTIFICADOR_USUARIO, context), out identificadorUsuario))
//            {
//                this.IdentificadorUsuario = identificadorUsuario;
//            }
//    	}
    
//    }
//}
