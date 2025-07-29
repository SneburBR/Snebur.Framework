using Snebur.Comunicacao.Dominio.RegraNegocio;
using Snebur.Dominio.Atributos;
using System.Reflection;

namespace Snebur.Comunicacao.Dominio.ServicoRegrasNegocio
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
