using Snebur.Dominio.Atributos;
using System.Collections.Generic;

namespace Snebur.Dominio
{
    [IgnorarInterfaceTS]
    public interface IEntidadeInterna : IEntidade
    {
        HashSet<string> __PropriedadesAbertas { get; }

        HashSet<string> __PropriedadesAutorizadas { get; }

        void AtivarControladorPropriedadeAlterada();

        void AtribuirPropriedadesAbertas(HashSet<string> PropriedadesAberta);

        void AtribuirPropriedadesAutorizadas(HashSet<string> PropriedadesAutorizadas);
    }
}