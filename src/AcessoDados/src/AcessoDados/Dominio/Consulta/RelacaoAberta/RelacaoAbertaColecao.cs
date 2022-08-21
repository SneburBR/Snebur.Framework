using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using System.Reflection;
using Snebur.Dominio.Atributos;

namespace Snebur.AcessoDados
{
    public class RelacaoAbertaColecao : BaseRelacaoAberta
    {
		#region Campos Privados

		#endregion

        public EstruturaConsulta EstruturaConsulta { get; set; } = new EstruturaConsulta();

        public RelacaoAbertaColecao() : base()
        {
        }
        [IgnorarConstrutorTS]
        public RelacaoAbertaColecao(PropertyInfo propriedade) : base(propriedade)
        {
        }
    }
}