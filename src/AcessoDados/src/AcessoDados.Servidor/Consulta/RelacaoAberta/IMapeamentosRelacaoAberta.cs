using Snebur.AcessoDados.Estrutura;
using System;

namespace Snebur.AcessoDados.Mapeamento
{
    internal interface IMapeamentosRelacaoAberta
    {
        Type TipoEntidade { get; set; }

        EstruturaEntidade EstruturaEntidade { get; set; }

        DicionarioEstrutura<MapeamentoConsultaRelacaoAberta> MapeamentosRelacaoAberta { get; set; }
    }
}