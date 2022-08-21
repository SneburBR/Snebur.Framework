using System.Reflection;
using Snebur.Comunicacao;
using Snebur.Dominio.Atributos;

namespace Snebur.AcessoDados
{
    public interface IServicoRegrasNegocio : IBaseServico
    {
        [IgnorarMetodoTS]
        object ChamarRegra(ChamadaRegraNegocio chamadaRegraNegocio, object[] parametros);
    }

    [IgnorarInterfaceTS]
    public interface IServicoRegrasNegocioCliente : IBaseServico
    {

        [IgnorarMetodoTS]
        object ChamarRegra(MethodBase metodo, ChamadaRegraNegocio chamadaRegra, object[] parametros);
    }
}
