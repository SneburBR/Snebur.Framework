using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using System.Reflection;

namespace Snebur.AcessoDados
{
    public class RelacaoAbertaEntidade : BaseRelacaoAberta
    {
		#region Campos Privados

		#endregion

        public RelacaoAbertaEntidade() : base()
        {
        }
        [IgnorarConstrutorTS]
        public RelacaoAbertaEntidade(PropertyInfo propriedade):base(propriedade)
        {
        }
    }
}