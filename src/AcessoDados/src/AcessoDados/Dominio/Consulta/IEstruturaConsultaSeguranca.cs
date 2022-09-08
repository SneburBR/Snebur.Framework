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
        HashSet<string> PropriedadesAutorizadas { get; }

        [IgnorarMetodoTSAttribute]
        void AtribuirPropriedadeAutorizadas(HashSet<string> propriedadesAutorizadas);
    }
}
