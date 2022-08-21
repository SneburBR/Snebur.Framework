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
    internal interface IMapeamentosRelacaoAberta
    {
        Type TipoEntidade { get; set; }

        EstruturaEntidade EstruturaEntidade { get; set; }

        DicionarioEstrutura<MapeamentoConsultaRelacaoAberta> MapeamentosRelacaoAberta { get; set; }
    }
}