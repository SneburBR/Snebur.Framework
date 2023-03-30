using Snebur.Dominio.Atributos;
using System.Collections.Generic;

namespace Snebur.AcessoDados.Seguranca
{
    public interface IEstruturaConsultaSeguranca
    {
        //[IgnorarPropriedadeTS]
        //[IgnorarPropriedadeTSReflexao]
        List<string> PropriedadesAbertas { get; set; }

        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        List<string> PropriedadesAutorizadas { get; }

        [IgnorarMetodoTS]
        void AtribuirPropriedadeAutorizadas(List<string> propriedadesAutorizadas);
    }
}
