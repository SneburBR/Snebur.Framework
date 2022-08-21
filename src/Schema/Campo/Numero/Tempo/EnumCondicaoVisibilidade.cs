using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Snebur.Utilidade;
using Snebur.Dominio;
using System.ComponentModel;

namespace Snebur.Schema
{
    
    public enum EnumCondicaoVisibilidade
    {
    	[Description("Desconhecido")]
    	Desconhecido = 0,

        [Description("Existe valor")]
        ExisteValor = 1,

        [Description("Mostrar")]
        Mostrar = 2,

        [Description("Notificar erro se nulo")]
        NotificaorErroSeNulo = 3,

        [Description("Ocultar")]
        Ocultar = 4
    }
}