using System.Collections.Generic;
using Snebur.Dominio.Atributos;

namespace Snebur.AcessoDados.Seguranca
{
    public interface IEstruturaConsultaSeguranca
    {
        //[IgnorarPropriedadeTS]
        //[IgnorarPropriedadeTSReflexao]
        List<string> PropriedadesAbertas { get; set; }

        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        HashSet<string> PropriedadesAutorizadas { get; }

        [IgnorarMetodoTSAttribute]
        void AtribuirPropriedadeAutorizadas(HashSet<string> propriedadesAutorizadas);
    }
}
