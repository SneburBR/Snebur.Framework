using Snebur.Dominio.Atributos;

namespace Snebur.AcessoDados
{
    [IgnorarInterfaceTS]
    public interface IAplicacaoServicoRegraNegocio
    {
        IServicoRegrasNegocioCliente ServicoRegrasNegocio { get; }
    }
}
