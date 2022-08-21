using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Snebur.Dominio.Atributos;

namespace Snebur.AcessoDados
{
    
    public abstract class BaseFiltroGrupo : BaseFiltro
    {
		#region Campos Privados

		#endregion

        public List<BaseFiltro> Filtros { get; set; } = new List<BaseFiltro>();
    }
}