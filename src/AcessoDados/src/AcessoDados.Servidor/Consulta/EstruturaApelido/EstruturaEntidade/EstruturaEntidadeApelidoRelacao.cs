using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Mapeamento
{
    internal abstract class EstruturaEntidadeApelidoRelacao : EstruturaEntidadeApelido
    {

        internal EstruturaRelacao EstruturaRelacao { get; set; }

        internal EstruturaEntidadeApelidoRelacao(BaseMapeamentoConsulta mapeamentoConsulta,
                                                 string apelidoEntidadeMapeada,
                                                 EstruturaEntidade estruturaEntidade,
                                                 EstruturaRelacao estruturaRelacao
                                                 ) :
                                                 base(mapeamentoConsulta,
                                                      apelidoEntidadeMapeada, 
                                                      estruturaEntidade)
        {
            this.EstruturaRelacao = estruturaRelacao;
        }
    }
}
